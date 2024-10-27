#if !UNITY_EDITOR
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;
using DrakiaXYZ.Hazardifier.Utils;
using EFT;
using HarmonyLib;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DrakiaXYZ.Hazardifier.Patches
{
    internal class MineInteractivePatch : ModulePatch
    {
        private static Type _playerActionClassType;
        private static Type _menuClassType;
        private static Type _menuItemClassType;
        private static Type _idleStateType;

        private static FieldInfo _menuItemNameField;
        private static FieldInfo _menuItemActionField;
        private static FieldInfo _menuActionsField;

        protected override MethodBase GetTargetMethod()
        {
            // Setup the class type references we'll need
            _playerActionClassType = PatchConstants.EftTypes.Single(x => x.GetMethods().Any(method => method.Name == "GetAvailableActions"));
            _menuClassType = PatchConstants.EftTypes.Single(x => x.GetMethod("SelectNextAction") != null);
            _menuItemClassType = PatchConstants.EftTypes.Single(x => x.GetField("TargetName") != null && x.GetField("Disabled") != null);
            _idleStateType = PatchConstants.EftTypes.Single(x =>
                AccessTools.GetDeclaredMethods(x).Any(method => method.Name == "Plant") &&
                AccessTools.GetDeclaredFields(x).Count >= 5 &&
                x.BaseType.Name == "MovementState"
            );

            // Get field accessors we'll need
            _menuItemNameField = AccessTools.Field(_menuItemClassType, "Name");
            _menuItemActionField = AccessTools.Field(_menuItemClassType, "Action");
            _menuActionsField = AccessTools.Field(_menuClassType, "Actions");

            return AccessTools.FirstMethod(_playerActionClassType, method =>
                    method.Name == "GetAvailableActions" &&
                    method.GetParameters()[0].ParameterType == typeof(GamePlayerOwner)
            );
        }

        [PatchPrefix]
        public static bool PatchPrefix(ref object __result, GamePlayerOwner owner, object interactive)
        {
            // If this is a MineInteractiveObject, create our menu
            MineInteractiveObject mineInteractiveObject;
            if ((mineInteractiveObject = interactive as MineInteractiveObject) != null)
            {
                // Create a new instance of the MenuClass
                object menu = Activator.CreateInstance(_menuClassType, new object[] { });

                // We can access a List<Type> as a generic list using IList
                IList menuItems = _menuActionsField.GetValue(menu) as IList;

                // Only create a menu item if the mine is armed, or we're allowed to re-arm it
                if (mineInteractiveObject.IsArmed() || Settings.AllowArming.Value)
                {
                    // Create the action handler
                    var actionHandler = new DisarmActionHandler
                    {
                        owner = owner,
                        mineInteractiveObject = mineInteractiveObject,
                        initialDistance = Vector3.Distance(owner.Player.Transform.position, mineInteractiveObject.transform.position)
                    };

                    // We can create an instance of a compile time unknown type using Activator
                    object menuItem = Activator.CreateInstance(_menuItemClassType, new object[] { });

                    // And then use FieldInfo objects to populate its
                    _menuItemNameField.SetValue(menuItem, mineInteractiveObject.IsArmed() ? "Disarm" : "Arm");
                    _menuItemActionField.SetValue(menuItem, new Action(actionHandler.DisarmMine));

                    // And because our list is generic, we can just insert into it like normal
                    menuItems.Insert(0, menuItem);
                }

                __result = menu;
                return false;
            }

            return true;
        }

        internal class DisarmActionHandler
        {
            public GamePlayerOwner owner;
            public MineInteractiveObject mineInteractiveObject;
            public float initialDistance;

            public void DisarmMine()
            {
                // Only allow disarming if the player is stationary
                if (_idleStateType.IsAssignableFrom(owner.Player.CurrentState.GetType()))
                {
                    // Show the timer panel
                    owner.ShowObjectivesPanel(mineInteractiveObject.IsArmed() ? "Disarming Mine {0:F1}" : "Arming Mine {0:F1}", Settings.MineDisarmTime.Value);

                    // Start the countdown, and trigger the ActionCompleteHandler when it's done
                    MovementState currentManagedState = owner.Player.CurrentManagedState;
                    float plantTime = Settings.MineDisarmTime.Value;
                    ActionCompleteHandler actionCompleteHandler = new ActionCompleteHandler()
                    {
                        owner = owner,
                        mineInteractiveObject = mineInteractiveObject
                    };
                    Action<bool> action = new Action<bool>(actionCompleteHandler.Complete);
                    currentManagedState.Plant(true, false, plantTime, action);
                }
                else
                {
                    if (mineInteractiveObject.IsArmed())
                    {
                        owner.DisplayPreloaderUiNotification("You can't disarm a mine while moving");
                    }
                    else
                    {
                        owner.DisplayPreloaderUiNotification("You can't arm a mine while moving");
                    }
                }
            }
        }

        internal class ActionCompleteHandler
        {
            public GamePlayerOwner owner;
            public MineInteractiveObject mineInteractiveObject;

            public void Complete(bool successful)
            {
                owner.CloseObjectivesPanel();
                if (successful)
                {
                    if (mineInteractiveObject.IsArmed())
                    {
                        mineInteractiveObject.DisarmMine();
                    }
                    else
                    {
                        mineInteractiveObject.ArmMine();
                    }
                }
            }
        }
    }
}
#endif
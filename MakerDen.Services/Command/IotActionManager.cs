using System.Collections.Generic;

namespace MakerDen.Command
{
    public static class IotActionManager
    {

        private static uint _actionErrors;
        public static uint TotalActions { get; private set; }

        public static uint ActionErrorCount
        {
            get { return _actionErrors; }
        }

        private static List<IComponent> actuators = new List<IComponent>();

        public static void AddComponent(IComponent actuator)
        {
            actuators.Add(actuator);
        }

        public static void RemoveComponent(IComponent actuator)
        {
            actuators.Remove(actuator);
        }

        public static string[] Action(IotAction action)
        {
            if (action.cmd == "hello") { return GetAllItemName(); }
            if (action.cmd == null || action.item == null) { return null; }
            ActionByName(action);
            return null;
        }

        private static string[] GetAllItemName()
        {
            string[] result = new string[actuators.Count];
            int i = 0;

            foreach (var actuator in actuators)
            {
                result[i] = actuator.Name;
                i++;
            }
            return result;
        }

        static void ActionByName(IotAction action)
        {
            foreach (var actuator in actuators)
            {
                if (actuator.Name == action.item)
                {
                    try
                    {
                        TotalActions++;
                        actuator.Action(action);
                    }
                    catch { _actionErrors++; }
                    break;
                }
            }
        }

        public static IComponent FindByName(string name)
        {
            string n = name.ToLower();

            foreach (var actuator in actuators)
            {
                if (actuator.Name.Length != 0 && actuator.Name == n)
                {
                    return actuator;
                }
            }
            return null;
        }
    }
}

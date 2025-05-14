namespace SmartHomeSystem
{
    public class SmartHome
    {
        public bool IsSecurityArmed { get; private set; }
        public decimal CurrentTemperature { get; private set; }
        public bool AreLightsOn { get; private set; }
        private bool _isSystemActive;
        private readonly decimal _minComfortTemp = 18.0m;
        private readonly decimal _maxComfortTemp = 25.0m;

        public bool IsSystemActive => _isSystemActive;

        public SmartHome()
        {
            _isSystemActive = false;
            IsSecurityArmed = false;
            CurrentTemperature = 20.0m;
            AreLightsOn = false;
        }

        public bool ActivateSystem()
        {
            if (!_isSystemActive)
            {
                _isSystemActive = true;
                return true;
            }
            return false;
        }

        public bool DeactivateSystem()
        {
            if (_isSystemActive && !IsSecurityArmed)
            {
                _isSystemActive = false;
                AreLightsOn = false;
                return true;
            }
            return false;
        }

        public bool ToggleSecurity()
        {
            if (!_isSystemActive) return false;

            IsSecurityArmed = !IsSecurityArmed;

            if (IsSecurityArmed)
            {
                AreLightsOn = false;
            }

            return true;
        }

        public bool AdjustTemperature(decimal newTemp)
        {
            if (!_isSystemActive) return false;

            if (newTemp < 10.0m || newTemp > 35.0m)
                return false;

            CurrentTemperature = newTemp;

            // Автоматически управляем светом в зависимости от температуры
            if (CurrentTemperature < _minComfortTemp || CurrentTemperature > _maxComfortTemp)
            {
                AreLightsOn = true;
            }

            return true;
        }

        public bool ToggleLights()
        {
            if (!_isSystemActive || IsSecurityArmed) return false;

            AreLightsOn = !AreLightsOn;
            return true;
        }

        public string GetSystemStatus()
        {
            string status = _isSystemActive ? "Active" : "Inactive";
            string security = IsSecurityArmed ? "Armed" : "Disarmed";
            string lights = AreLightsOn ? "On" : "Off";

            // Исправленный формат температуры (без °C)
            return $"System: {status}, Security: {security}, " +
                   $"Temp: {CurrentTemperature}, Lights: {lights}";
        }
    }
}
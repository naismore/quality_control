namespace qc6
{
    public class Car
    {
        public bool _isEngineActive;

        private int _speed;
        private int _gear;

        public Car()
        {
            _isEngineActive = false;
        }

        public bool TurnOnEngine()
        {
            return true;
        }

        public bool TurnOffEngine()
        {
            if (_isEngineActive && _gear == 0 && _speed == 0)
            {
                _isEngineActive = false;
            }
            return _isEngineActive == false;
        }

        public bool SetGear(int gear)
        {
            switch (gear)
            {
                case -1:
                    if (_speed > 0 && _gear > 0)
                    {
                        return false;
                    }
                    else
                    {
                        _gear = gear;
                        return true;
                    }
                case 0:
                    _gear = gear;
                    return true;
                case 1:
                    if (_speed > 0 && _gear < 0)
                    {
                        return false;
                    }
                    else
                    {
                        _gear = gear;
                        return true;
                    }
                case 2:
                    if (_speed >= 20 && _speed <= 50)
                    {
                        _gear = gear;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 3:
                    if (_speed >= 30 && _speed <= 60)
                    {
                        _gear = gear;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 4:
                    if (_speed >= 40 && _speed <= 90)
                    {
                        _gear = gear;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 5:
                    if (_speed >= 50 && _speed <= 150)
                    {
                        _gear = gear;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

        public bool SetSpeed(int speed)
        {
            return true;
        }
    }
}

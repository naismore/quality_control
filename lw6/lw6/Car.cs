class Car
{
    private string _name = "Undefined";
    private string _model = "Undefined";
    private float _horsePoints = 0;

    private bool _engineOn = false;

    private int _gear = 0;

    public string Name
    {
        get => _name; set
        {
            if (_name != value && !string.IsNullOrEmpty(value))
            {
                _name = value;
            }
        }
    }

    public string Model
    {
        get => _model;
        set
        {
            if (_model != value && !string.IsNullOrEmpty(value))
            {
                _model = value;
            }
        }
    }

    public float HorsePoints
    {
        get => _horsePoints;
        set
        {
            if (_horsePoints != value && value > 0)
            {
                _horsePoints = value;
            }
        }
    }

    public Car(string name, string model, float horsePoints)
    {
        _name = name;
        _model = model;
        _horsePoints = horsePoints;
    }

    public void SetGear()
    {
        
    }

}
[System.Serializable]
public class PidController
{
    public readonly float pFactor, iFactor, dFactor;
    private float integral;
    private float lastError;

    public PidController(float pFactor = 0.1f, float iFactor = 0.01f, float dFactor = 0.01f)
    {
        this.pFactor = pFactor; // (proportional) weighting of the current difference between target and actual.
        this.iFactor = iFactor; // (integral) weighting of error that accumulates over time/'steady state error' eg. due to an external problem such as being knocked off course by hitting another body.
        this.dFactor = dFactor; // (derivative) weighting of the rate of change of error - anticipates error and reduces overshoot caused by P. Higher D factor dampens the response to prevent oscillations.
    }

    // public float Update(float target, float actual, float timeDelta)
    // {
    //     float error = target - actual;
    //     integral += error * timeDelta;
    //     float derivative = (error - lastError) / timeDelta;
    //     lastError = error;

    //     return
    //         error * pFactor
    //         + integral * iFactor
    //         + derivative * dFactor;
    // }

    public float LiveTuneUpdate(float target, float actual, float timeDelta, float p, float i, float d)
    {
        float error = target - actual;
        integral += error * timeDelta;
        float derivative = (error - lastError) / timeDelta;
        lastError = error;

        return
            p * error +
            i * integral +
            d * derivative;
    }
}

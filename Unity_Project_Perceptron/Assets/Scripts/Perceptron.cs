using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TrainingSet
{
    public double[] input;
    public double output;
}

public class Perceptron : MonoBehaviour
{

    public float Magnifier = 400;
    public int Trainings_pSession = 8;

    public TMP_InputField input_traningFreq;

    public TMP_InputField input_Para1;
    public TMP_InputField input_Para2;

    public TextMeshProUGUI Weight1TEXT;
    public TextMeshProUGUI Weight2TEXT;
    public TextMeshProUGUI BiasTEXT;
    public TextMeshProUGUI TotErrorTEXT;

    public TrainingSet[] trainingset_variable;
    double[] weights = { 0, 0 };
    double bias = 0;
    double totalError = 0;

    //public SimpleGrapher sg;
    public Window_Graph GraphPlotter_gp;
    public GameObject boundaryLine;

    //The function that will be trained for the classification
    double DotProductBias(double[] v1, double[] v2)
    {
        if (v1 == null || v2 == null)
            return -1;

        if (v1.Length != v2.Length)
            return -1;

        double d = 0;
        for (int x = 0; x < v1.Length; x++)
        {
            d += v1[x] * v2[x];
        }

        d += bias;

        return d;
    }

    //Explicity calculate the output afor classification after training
    double CalcOutput(int i)
    {
        double dp = DotProductBias(weights, trainingset_variable[i].input);
        if (dp > 0) return (1);
        return (0);
    }

    void InitialiseWeights()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(-1.0f, 1.0f);
        }
        bias = Random.Range(-1.0f, 1.0f);
    }

    void UpdateWeights(int j)
    {
        double error = trainingset_variable[j].output - CalcOutput(j);
        totalError += Mathf.Abs((float)error);
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = weights[i] + error * trainingset_variable[j].input[i];
        }
        bias += error;
    }

    //For sorting the data after training has been done
    double CalcOutput(double i1, double i2)
    {
        double[] inp = new double[] { i1, i2 };
        double dp = DotProductBias(weights, inp);
        if (dp > 0) return (1);
        return (0);
    }

    void Train(int epochs)
    {
        InitialiseWeights();

        for (int e = 0; e < epochs; e++)
        {
            totalError = 0;
            for (int t = 0; t < trainingset_variable.Length; t++)
            {
                UpdateWeights(t);
                Debug.Log("W1: " + (weights[0]) + " W2: " + (weights[1]) + " B: " + bias);

                Weight1TEXT.text = "WEIGHT1\n" + weights[0].ToString("F3");
                Weight2TEXT.text = "WEIGHT2\n" + weights[1].ToString("F3");
                BiasTEXT.text = "BIAS\n" + bias.ToString("F3");
            }

            Debug.Log("TOTAL ERROR: " + totalError);
            TotErrorTEXT.text = "TOTAL ERRORS ENCOUNTERED\n" + totalError.ToString("F3");
        }
    }

    void DrawAllPoints()
    {
        //Draws all points
        //DrawPoint(x_value,y_value,Color)

        for (int t = 0; t < trainingset_variable.Length; t++)
        {
            if (trainingset_variable[t].output == 0)
                GraphPlotter_gp.CreateCircle(Magnifier*(float)trainingset_variable[t].input[0], Magnifier*(float)trainingset_variable[t].input[1], Color.magenta);
            else
                GraphPlotter_gp.CreateCircle(Magnifier*(float)trainingset_variable[t].input[0], Magnifier*(float)trainingset_variable[t].input[1], Color.green);
        }
    }

    void Start()
    {
        DrawAllPoints();
        Train(Trainings_pSession);

        //Determine the two points
        //y = mx + c

        float m = (float)(-(bias / weights[1]) / (bias / weights[0])); //slope
        float c = (float)(-bias / weights[1]);

        Vector2 PointA = new Vector2(0, Magnifier*(float)(-bias / weights[1]));
        Vector2 PointB = new Vector2(5*Magnifier, (float)(m * (5*Magnifier) + c));

        boundaryLine = GraphPlotter_gp.CreateDotConnection(PointA, PointB, Color.blue); //Assign to delete at runtime

    }

    public void ButtonMethodCall()
    {
        //Redo steps in Start for a new training interval during runtime

        if(input_traningFreq.text!=null)
        {
            //Debug.Log("Parsing int");

            //Get the number from the input text
            Trainings_pSession = int.Parse(input_traningFreq.text);

            //if we get a positive number then, delete previous graph line
            Destroy(boundaryLine);
        }

        //DrawAllPoints();
        //All the circles are already plotted so DrawAllPointsNotNeeded

        Train(Trainings_pSession);

        //Determine the two points
        //y = mx + c

        float m = (float)(-(bias / weights[1]) / (bias / weights[0])); //slope
        float c = (float)(-bias / weights[1]);

        Vector2 PointA = new Vector2(0,c);
        Vector2 PointB = new Vector2(2 * Magnifier, (float)(m * (2 * Magnifier) + c));

        boundaryLine = GraphPlotter_gp.CreateDotConnection(PointA, PointB, Color.blue);
    }

    public void ExtraPointFunction()
    {
        //Redo steps in Start for a new training interval during runtime
        float para1=0;
        float para2=0;

        if(input_Para1.text!=null && input_Para2.text!=null)
        {
            //Debug.Log("Parsing int");

            //Get the number from the input text
            Trainings_pSession = int.Parse(input_traningFreq.text);

            para1 = float.Parse(input_Para1.text);
            para2 = float.Parse(input_Para2.text);

            //if we get a positive number then, delete previous graph line
            Destroy(boundaryLine);
        }

        Train(Trainings_pSession);

        float m = (float)(-(bias / weights[1]) / (bias / weights[0])); //slope
        float c = (float)(-bias / weights[1]);

        Vector2 PointA = new Vector2(0,c);
        Vector2 PointB = new Vector2(2 * Magnifier, (float)(m * (2 * Magnifier) + c));

        boundaryLine = GraphPlotter_gp.CreateDotConnection(PointA, PointB, Color.blue);

        if(CalcOutput(para1,para2)==0)
        {
            //It's a bad steel
            GraphPlotter_gp.CreateCircle(Magnifier* para1, Magnifier * para2, Color.red);
        }
        else
        {
            GraphPlotter_gp.CreateCircle(Magnifier * para1, Magnifier * para2, Color.yellow);
        }

    }

}
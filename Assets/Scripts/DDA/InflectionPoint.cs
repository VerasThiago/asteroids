using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflectionPoint {

    public enum ExtremaType
    {
        Minimum,
        Maximum,
        None
    }

    private double x;
    private double y;
    private int index;
    private ExtremaType extremumType;

    public InflectionPoint(double x1, double y1, int indexOrigin)
    {
        this.x = x1;
        this.y = y1;
        this.index = indexOrigin;
    }

    public InflectionPoint(EDASignal signalValue, int indexOrigin, ExtremaType extremum)
    {
        this.x = signalValue.time;
        this.y = signalValue.value;
        this.index = indexOrigin;
        this.extremumType = extremum;
    }


    public ExtremaType ExtremumType
    {
        get
        {
            return extremumType;
        }
        set
        {
            extremumType = value;
        }
    }

    public int IndexOrigin
    {
        get
        {
            return index;
        }
        set
        {
            x = value;
        }
    }

    public double CoordinateX
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
        }
    }

    public double CoordinateY
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
        }
    }
}

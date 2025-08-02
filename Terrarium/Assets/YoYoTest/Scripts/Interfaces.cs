using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//define a interface for the object
public interface IGetObjectClass
{
    string BigClass { get; }
    string SmallClass { get; }
}

public interface IGrowth
{
    //生长速度
    float GrowthSpeed { get; set; }
    //生长进度
    float GrowthProgress { get; set; }
    //生长
    void Growth();
}

public interface IReleaseResearchPoint
{
    void ReleaseResearchPoint();
}

public interface IReleaseSeed
{
    void ReleaseSeed();
}

public interface IReproduction
{
    void Reproduction();
}



public interface IGetGroundClass
{
    int Fertility { get; }
}






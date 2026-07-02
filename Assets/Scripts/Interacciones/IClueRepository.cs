using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClueRepository
{
    void Add(Clue clue);
    IReadOnlyList<Clue> GetAll();
}
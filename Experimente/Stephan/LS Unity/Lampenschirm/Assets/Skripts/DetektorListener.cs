using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetektorListener
{
    void recv_last_detection((float, float) last);
}

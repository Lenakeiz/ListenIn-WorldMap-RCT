using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface AIState<T>
{
    string Name { get; set; }
    void EnterState(T agent);
    void ExecuteState(T agent);
    void ExitState(T agent);
}


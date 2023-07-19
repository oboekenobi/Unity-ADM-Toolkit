using System;
using System.Linq;
using UnityEngine.Playables;
using System.Collections.Generic;

using Obj = UnityEngine.Object;
using UnityEngine;
using UnityEngine.Timeline;

public static class PlayableUtilities
{
    /*public static void GetLastBehaviourAndBinding<T>(PlayableDirector director, Dictionary<Obj, T> behavioursAndBindings) where T : class, IPlayableBehaviour, new()
    {
        Dictionary<Obj, List<T>> behavioursAndBindingsTmp = new Dictionary<Obj, List<T>>();
        GetBehavioursAndBindings(director, behavioursAndBindingsTmp);

        foreach (var behavioursAndBinding in behavioursAndBindingsTmp)
            behavioursAndBindings.Add(behavioursAndBinding.Key, behavioursAndBinding.Value.LastOrDefault());
    }*/

    public static void SortBehaviourBindings(PlayableDirector director)
    {
        
    }
    public static void GetBehavioursAndBindings<T>(PlayableDirector director, List<T> behavioursAndBindings) where T : class, IPlayableBehaviour, new()
    {
        PlayableGraph playableGraph = director.playableGraph;

        if (!playableGraph.IsValid())
        {
            return;
        }

        int numOutputs = playableGraph.GetOutputCount();
        for (int i = 0; i < numOutputs; i++)
        {
            PlayableOutput output = playableGraph.GetOutput(i);

            if (!output.IsOutputValid() || !output.IsPlayableOutputOfType<ScriptPlayableOutput>())
            {
                continue;
            }

            int sourceOutputPort = output.GetSourceOutputPort();
            Playable playable = output.GetSourcePlayable().GetInput(sourceOutputPort);

            List<T> behaviours = new List<T>();
            GetBehaviours(playable, behaviours);


            Obj referenceObject = output.GetReferenceObject();
            Obj binding = null;

            if (referenceObject != null)
                binding = director.GetGenericBinding(referenceObject);

            /*if (behaviours.Count > 0)
                behavioursAndBindings.Add(binding, behaviours);*/

            for (int j = 0; j < behaviours.Count; i++)
            {
                behavioursAndBindings.Add(behaviours[i]);
            }
        }
    }

    public static void GetBehaviours<T>(PlayableGraph playableGraph, List<T> behaviours) where T : class, IPlayableBehaviour, new()
    {
        if (!playableGraph.IsValid())
        {
            return;
        }

        int numOutputs = playableGraph.GetOutputCount();
        for (int i = 0; i < numOutputs; i++)
        {
            PlayableOutput output = playableGraph.GetOutput(i);

            if (!output.IsOutputValid() || !output.IsPlayableOutputOfType<ScriptPlayableOutput>())
            {
                continue;
            }

            int sourceOutputPort = output.GetSourceOutputPort();
            Playable playable = output.GetSourcePlayable().GetInput(sourceOutputPort);

            GetBehaviours(playable, behaviours);
        }
    }

    public static void GetBehaviours<T>(Playable playable, List<T> behaviours) where T : class, IPlayableBehaviour, new()
    {
        if (!playable.IsValid())
        {
            return;
        }

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            Type behaviourType = typeof(T);
            Playable input = playable.GetInput(i);

            if (input.GetInputCount() > 0)
            {
                GetBehaviours(input, behaviours);
            }
            else if (input.GetPlayableType() == behaviourType)
            {
                T behaviour = ((ScriptPlayable<T>)input).GetBehaviour();

                if (!behaviours.Contains(behaviour))
                {
                    behaviours.Add(behaviour);
                }
            }
        }
    }
}

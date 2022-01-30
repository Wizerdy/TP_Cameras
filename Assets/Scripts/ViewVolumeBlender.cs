using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewVolumeBlender
{
    private static ViewVolumeBlender instance;

    private List<AViewVolume> ActiveViewVolumes = new List<AViewVolume>();
    private Dictionary<AView, List<AViewVolume>> VolumesPerViews = new Dictionary<AView, List<AViewVolume>>();

    public static ViewVolumeBlender Instance {
        get { if (instance == null) instance = new ViewVolumeBlender(); return instance; }
    }

    public void Awake() {
        ActiveViewVolumes = new List<AViewVolume>();
        VolumesPerViews = new Dictionary<AView, List<AViewVolume>>();
    }

    public void Update() {
        WeightByPriority();
    }

    public void WeightByPriority()
    {
        List<int> priorities = new List<int>();
        Dictionary<int, float> weightSumPerPriority = new Dictionary<int, float>();
        Dictionary<int, float> maxWeightPerPriority = new Dictionary<int, float>();

        foreach (AViewVolume viewVolume in ActiveViewVolumes)
        {
            if (viewVolume == null) { continue; }

            viewVolume.view.Weight = 0;
            if (!priorities.Contains(viewVolume.Priority))
            {
                priorities.Add(viewVolume.Priority);
                weightSumPerPriority.Add(viewVolume.Priority, 0f);
                maxWeightPerPriority.Add(viewVolume.Priority, 0f);
            }

            float weight = viewVolume.ComputeSelfWeight();
            weightSumPerPriority[viewVolume.Priority] += weight;
            if (weight > maxWeightPerPriority[viewVolume.Priority])
            {
                maxWeightPerPriority[viewVolume.Priority] = weight;
            }
        }

        Dictionary<int, float> priorityWeights = new Dictionary<int, float>();
        float availableWeight = 1f;

        priorities.Sort();
        priorities.Reverse();

        foreach (int priority in priorities)
        {
            priorityWeights[priority] = availableWeight * maxWeightPerPriority[priority];
            availableWeight *= (1f - maxWeightPerPriority[priority]);
        }

        foreach (AViewVolume v in ActiveViewVolumes)
        {
            v.view.Weight = v.ComputeSelfWeight() * priorityWeights[v.Priority] / weightSumPerPriority[v.Priority];
        }
    }

    public void PriorityFilter()
    {
        float maxPrio = 0f;

        foreach (AViewVolume viewVolume in ActiveViewVolumes)
        {
            if (maxPrio < viewVolume.Priority)
            {
                maxPrio = viewVolume.Priority;
            }
            viewVolume.view.Weight = 0;
        }

        foreach (AViewVolume v in ActiveViewVolumes)
        {
            if (v.Priority < maxPrio)
            {
                v.view.Weight = 0;
            }
            else
            {
                v.view.Weight = Mathf.Max(v.view.Weight, v.ComputeSelfWeight());
            }
        }
    }

    public void AddVolume(AViewVolume volumeToAdd)
    {
        ActiveViewVolumes.Add(volumeToAdd);
        bool addView = true;
        foreach (var item in VolumesPerViews)
        {
            foreach (AViewVolume volume in item.Value)
            {
                if (volume == volumeToAdd)
                {
                    addView = false;
                }
            }
        }
        if (addView)
        {
            List<AViewVolume> newList = new List<AViewVolume>();
            newList.Add(volumeToAdd);
            VolumesPerViews.Add(volumeToAdd.view, newList);
            volumeToAdd.view.SetActive(true);
        }
    }

    public void RemoveVolume(AViewVolume volumeToRemove)
    {
        ActiveViewVolumes.Remove(volumeToRemove);
        AView viewToDelet = null;
        foreach (var item in VolumesPerViews) {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (item.Value[i] == volumeToRemove)
                {
                    item.Value.RemoveAt(i);
                    if(item.Value.Count == 0)
                    {
                        viewToDelet = item.Key;
                    }

                }
            }
        }
        if (viewToDelet != null)
        {
            viewToDelet.SetActive(false);
            VolumesPerViews.Remove(viewToDelet);

        }
    }
}

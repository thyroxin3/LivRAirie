using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]

public class PlaceTrackedImages : MonoBehaviour
{
    private ARTrackedImageManager _trackedImagesManager;

    //List of prefabs to instantiate - these should be named the same as their correspoding 2D images in the reference image library

    public GameObject[] ArPrefabs;

    //Keep dictionary array of created prefabs
    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary <string, GameObject> ();
    // Start is called before the first frame update
    void Awake()
    {
        // Cache a reference to the tracked Image Manager component
        _trackedImagesManager = GetComponent<ARTrackedImageManager>();

    }
    void OnEnable()
    {
        //Attach event handler when tracled images change
        _trackedImagesManager.trackedImagesChanged += OnTrackerdImagesChanged;
    }
    void OnDisable()
    {
        //Remove event handler
        _trackedImagesManager.trackedImagesChanged -= OnTrackerdImagesChanged;
    }
    //Event Handler
    private void OnTrackerdImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //Loop through all new tracked images that have been detected
        foreach (var trackedImage in eventArgs.added)
        {
            //Get the name of the reference image
            var imageName = trackedImage.referenceImage.name;
            //Now loop over the array of prefabs
            foreach (var curPrefab in ArPrefabs)
            {
                //Check whether this prefab matches the tracked image name, and that the prefab hasnt already been created
                if (string.Compare(curPrefab.name, imageName, System.StringComparison.OrdinalIgnoreCase) == 0 && !_instantiatedPrefabs.ContainsKey(imageName))
                {
                    //Instantiate the prefab, parenting it to the ARTrackedImage
                    var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    // Add the created prefab to our array
                    _instantiatedPrefabs[imageName] = newPrefab;
                }
            }
        }
    

    //For all prefabs that have been created so far, set active or not depending on whether their corresponding image is currently being tracked
    foreach (var trackedImage in eventArgs.updated) { _instantiatedPrefabs[trackedImage.referenceImage.name] 
                .SetActive(trackedImage.trackingState == TrackingState.Tracking); 
        }


//If hte AR subsystem has given up looking for a tracked image
foreach (var trackedImage in eventArgs.removed)
{
    //Destroy its prefab
    Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
    //Also remove the instance  from our array
    _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
    // Or simply set the prefab instance to inactive 
}
}
}

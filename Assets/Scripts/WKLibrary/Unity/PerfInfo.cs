using UnityEngine;
using System.Collections;
using System.Text;
 
namespace WK { namespace Unity {

[ExecuteInEditMode ()]
public class PerfInfo : SingletonMonoBehaviour<PerfInfo> {
    public int  x            = 5;
    public int  y            = 5;
	public bool show         = true;
	public bool showMemory   = false;
	public bool showFPS      = false;
	public bool showInEditor = false;

	public void Start () {
		useGUILayout = false;
	}
 
	public void OnGUI () {
        if (!UnityEngine.Debug.isDebugBuild) return;

		if (!show || (!Application.isPlaying && !showInEditor)) {
			return;
		}
 
		int collCount = System.GC.CollectionCount (0);
 
		if (lastCollectNum != collCount) {
			lastCollectNum = collCount;
			delta = Time.realtimeSinceStartup-lastCollect;
			lastCollect = Time.realtimeSinceStartup;
			lastDeltaTime = Time.deltaTime;
			collectAlloc = allocMem;
		}
 
		allocMem = (int)System.GC.GetTotalMemory (false);
 
		peakAlloc = allocMem > peakAlloc ? allocMem : peakAlloc;
 
		if (Time.realtimeSinceStartup - lastAllocSet > 0.3F) {
			int diff = allocMem - lastAllocMemory;
			lastAllocMemory = allocMem;
			lastAllocSet = Time.realtimeSinceStartup;
 
			if (diff >= 0) {
				allocRate = diff;
			}
		}
 
		StringBuilder text = new StringBuilder ();
        int height = 32;
 
        appendText(ref text, ref height,
                "Quality Level : "
                + QualitySettings.GetQualityLevel().ToString()
                + "\n"
                );

		if (showMemory) {
            appendText(ref text, ref height, "Currently allocated   " 
                    + (allocMem/1000000F).ToString ("0")
                    + "mb\n");

            appendText(ref text, ref height, "Peak allocated   "
                    + (peakAlloc/1000000F).ToString ("0")
                    + "mb \n(last collect ");

            appendText(ref text, ref height, (collectAlloc/1000000F).ToString ("0")
                    + " mb)\n");

            appendText(ref text, ref height, "Allocation rate   "
                    + (allocRate/1000000F).ToString ("0.0")
                    + "mb\n");

            appendText(ref text, ref height, "Collection frequency   "
                    + delta.ToString ("0.00")
                    + "s\n");

            appendText(ref text, ref height, "Last collect delta   "
                    + lastDeltaTime.ToString ("0.000")
                    + "s ("
                    + (1F/lastDeltaTime).ToString ("0.0")
                    + " fps)\n");
        }
 
		if (showFPS) {
			appendText(ref text, ref height, (1F/Time.deltaTime).ToString ("0.0")+" fps");
		}
 
        const int cMergin = 5;
        const int cWidth = 240;
		GUI.Box (new Rect (x,y,cWidth,height),"");
		/* GUI.Label (new Rect (x+cMergin,y+cMergin,cWidth,height),text.ToString ()); */
		GUI.Label (new Rect (x+cMergin,y,cWidth,height),text.ToString ());
	}

    private void appendText( ref StringBuilder text, ref int height, string str )
    {
        const int cOneLineHeight = 12;
		text.Append ( str );
        height += cOneLineHeight;
    }

	private float lastCollect    = 0;
	private float lastCollectNum = 0;
	private float delta          = 0;
	private float lastDeltaTime  = 0;
	private int allocRate        = 0;
	private int lastAllocMemory  = 0;
	private float lastAllocSet   = -9999;
	private int allocMem         = 0;
	private int collectAlloc     = 0;
	private int peakAlloc        = 0;
}

}}

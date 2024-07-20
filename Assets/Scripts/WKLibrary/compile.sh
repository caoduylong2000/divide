#!/bin/bash
/Applications/Unity/Unity.app/Contents/Frameworks/Mono/bin/smcs -r:/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll \
-r:/Applications/Unity/Unity.app/Contents/UnityExtensions/Unity/GUISystem/Standalone/UnityEngine.UI.dll \
-r:/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll \
-target:library -out:$1 **/*.cs


using UnityEngine;
using UnityEditor;
using System.IO;

namespace Canicross.Editor
{
    public static class WebGLBuilder
    {
        [MenuItem("Canicross/Build WebGL")]
        public static void BuildWebGL()
        {
            string buildPath = Path.GetDirectoryName(Application.dataPath);
            buildPath = Path.Combine(buildPath, "Builds", "WebGL");

            Directory.CreateDirectory(buildPath);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/_Game/Scenes/MainMenu.unity",
                    "Assets/_Game/Scenes/Track_01_Lago.unity"
                },
                locationPathName = buildPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[Canicross] WebGL build succeeded: {summary.totalSize / 1024 / 1024} MB");
                CreateIndexOverride(buildPath);
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("[Canicross] WebGL build failed!");
            }
        }

        private static void CreateIndexOverride(string buildPath)
        {
            string indexPath = Path.Combine(buildPath, "index.html");
            if (!File.Exists(indexPath)) return;

            string customIndex = GetCustomIndexHtml();
            File.WriteAllText(indexPath, customIndex);
            Debug.Log("[Canicross] Custom index.html created");
        }

        private static string GetCustomIndexHtml()
        {
            return @"<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""utf-8"">
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, user-scalable=no, viewport-fit=cover"">
    <meta name=""apple-mobile-web-app-capable"" content=""yes"">
    <meta name=""mobile-web-app-capable"" content=""yes"">
    <title>Canicross</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; -webkit-tap-highlight-color: transparent; }
        html, body { width: 100%; height: 100%; overflow: hidden; background: #0a0a1a; touch-action: none; }
        #unity-container { width: 100%; height: 100%; position: relative; }
        #unity-canvas { width: 100%; height: 100%; display: block; }
        
        #unity-loading-bar { 
            position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); 
            text-align: center; color: white; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; 
        }
        #unity-loading-bar .title { font-size: 28px; font-weight: bold; margin-bottom: 8px; color: #DAA520; }
        #unity-loading-bar .subtitle { font-size: 14px; opacity: 0.7; margin-bottom: 20px; }
        #unity-progress-bar { width: 240px; height: 6px; background: rgba(255,255,255,0.15); border-radius: 3px; margin: 0 auto; overflow: hidden; }
        #unity-progress-fill { height: 100%; background: linear-gradient(90deg, #00BFFF, #60C0E0); width: 0%; transition: width 0.2s; border-radius: 3px; }
        
        #voice-overlay { 
            position: absolute; bottom: 0; left: 0; right: 0; 
            background: linear-gradient(to top, rgba(0,0,0,0.8), transparent); 
            padding: 30px 20px 40px; pointer-events: none; opacity: 0; transition: opacity 0.5s;
            display: flex; flex-direction: column; align-items: center; gap: 10px;
        }
        #voice-overlay.visible { opacity: 1; }
        #voice-overlay .hint { color: rgba(255,255,255,0.9); font-family: sans-serif; font-size: 15px; text-align: center; }
        #voice-overlay .commands { color: #00BFFF; font-family: sans-serif; font-size: 13px; text-align: center; }
        
        #mic-button { 
            position: absolute; bottom: 20px; right: 20px; width: 56px; height: 56px; 
            border-radius: 50%; border: 2px solid rgba(255,255,255,0.3); 
            background: rgba(0,0,0,0.5); color: white; font-size: 24px; cursor: pointer; 
            display: flex; align-items: center; justify-content: center;
            transition: all 0.3s; pointer-events: auto; z-index: 10;
        }
        #mic-button:active { transform: scale(0.95); }
        #mic-button.active { background: #00BFFF; border-color: #00BFFF; }
        #mic-button.active::after { content: ''; position: absolute; width: 100%; height: 100%; border-radius: 50%; border: 2px solid #00BFFF; animation: pulse 1.5s infinite; }
        
        @keyframes pulse {
            0% { transform: scale(1); opacity: 1; }
            100% { transform: scale(1.6); opacity: 0; }
        }
        
        #touch-controls {
            position: absolute; bottom: 20px; left: 20px;
            display: none; gap: 10px; z-index: 10;
        }
        .touch-btn {
            width: 70px; height: 70px; border-radius: 50%;
            background: rgba(0,0,0,0.4); border: 2px solid rgba(255,255,255,0.3);
            color: white; font-size: 24px; display: flex; align-items: center; justify-content: center;
            user-select: none; -webkit-user-select: none; touch-action: manipulation;
        }
        .touch-btn:active { background: rgba(0,191,255,0.5); }
        
        @media (max-width: 768px) {
            #touch-controls { display: flex; }
        }
        
        .rotate-hint {
            display: none; position: fixed; top: 0; left: 0; right: 0; bottom: 0;
            background: #0a0a1a; z-index: 100; justify-content: center; align-items: center;
            flex-direction: column; color: white; font-family: sans-serif; text-align: center; padding: 20px;
        }
        @media (max-width: 768px) and (orientation: portrait) {
            .rotate-hint { display: flex; }
        }
    </style>
</head>
<body>
    <div class=""rotate-hint"">
        <div style=""font-size: 48px; margin-bottom: 16px;"">&#x1F4F1;</div>
        <div style=""font-size: 20px; font-weight: bold; margin-bottom: 8px;"">Gire seu celular</div>
        <div style=""font-size: 14px; opacity: 0.7;"">Para uma melhor experiencia, use o modo paisagem</div>
    </div>
    
    <div id=""unity-container"">
        <canvas id=""unity-canvas""></canvas>
        
        <div id=""unity-loading-bar"">
            <div class=""title"">CANICROSS</div>
            <div class=""subtitle"">Corra com seu cao pela trilha</div>
            <div id=""unity-progress-bar""><div id=""unity-progress-fill""></div></div>
        </div>
        
        <div id=""voice-overlay"">
            <div class=""hint"">Toque no microfone e fale:</div>
            <div class=""commands"">DIREITA | ESQUERDA | VAI | PULA</div>
        </div>
        
        <div id=""touch-controls"">
            <div class=""touch-btn"" id=""btn-left"">&#x25C0;</div>
            <div class=""touch-btn"" id=""btn-right"">&#x25B6;</div>
        </div>
        
        <button id=""mic-button"">&#x1F3A4;</button>
    </div>
    
    <script>
        const canvas = document.getElementById('unity-canvas');
        const loadingBar = document.getElementById('unity-loading-bar');
        const progressFill = document.getElementById('unity-progress-fill');
        const voiceOverlay = document.getElementById('voice-overlay');
        const micButton = document.getElementById('mic-button');
        
        // Touch controls for mobile
        const btnLeft = document.getElementById('btn-left');
        const btnRight = document.getElementById('btn-right');
        
        let touchLateral = 0;
        
        if (btnLeft && btnRight) {
            btnLeft.addEventListener('touchstart', (e) => { e.preventDefault(); touchLateral = -1; });
            btnLeft.addEventListener('touchend', (e) => { e.preventDefault(); touchLateral = 0; });
            btnRight.addEventListener('touchstart', (e) => { e.preventDefault(); touchLateral = 1; });
            btnRight.addEventListener('touchend', (e) => { e.preventDefault(); touchLateral = 0; });
        }
        
        // Send touch input to Unity
        setInterval(() => {
            if (window.unityInstance && touchLateral !== 0) {
                window.unityInstance.SendMessage('VoiceInput', 'SetTouchInput', touchLateral);
            }
        }, 50);
        
        const buildUrl = 'Build';
        const loaderUrl = buildUrl + '/WebGL.loader.js';
        
        const config = {
            dataUrl: buildUrl + '/WebGL.data.unityweb',
            frameworkUrl: buildUrl + '/WebGL.framework.js.unityweb',
            codeUrl: buildUrl + '/WebGL.wasm.unityweb',
            streamingAssetsUrl: 'StreamingAssets',
            companyName: 'Canicross',
            productName: 'Canicross',
            productVersion: '0.1',
            showBanner: function(msg) { console.log(msg); }
        };
        
        const script = document.createElement('script');
        script.src = loaderUrl;
        script.onload = () => {
            createUnityInstance(canvas, config, (progress) => {
                progressFill.style.width = (progress * 100) + '%';
            }).then((unityInstance) => {
                window.unityInstance = unityInstance;
                loadingBar.style.display = 'none';
                voiceOverlay.classList.add('visible');
                
                // Microphone toggle
                let micActive = false;
                micButton.addEventListener('click', () => {
                    micActive = !micActive;
                    micButton.classList.toggle('active', micActive);
                    unityInstance.SendMessage('VoiceInput', 'ToggleListening');
                });
            }).catch((message) => {
                console.error(message);
            });
        };
        document.body.appendChild(script);
    </script>
</body>
</html>";
        }
    }
}

mergeInto(LibraryManager.library, {
    VoiceControl_Init: function(callbackObjectName, callbackMethodName) {
        if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
            console.warn('[VoiceControl] Web Speech API not supported in this browser');
            return 0;
        }
        
        var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        window.voiceRecognition = new SpeechRecognition();
        window.voiceRecognition.continuous = true;
        window.voiceRecognition.interimResults = false;
        window.voiceRecognition.lang = 'pt-BR';
        
        window.voiceCallbackObject = Pointer_stringify(callbackObjectName);
        window.voiceCallbackMethod = Pointer_stringify(callbackMethodName);
        
        window.voiceRecognition.onresult = function(event) {
            var lastResult = event.results[event.results.length - 1];
            if (lastResult.isFinal) {
                var transcript = lastResult[0].transcript.trim().toLowerCase();
                console.log('[VoiceControl] Recognized: ' + transcript);
                
                if (window.unityInstance) {
                    window.unityInstance.SendMessage(
                        window.voiceCallbackObject,
                        window.voiceCallbackMethod,
                        transcript
                    );
                }
            }
        };
        
        window.voiceRecognition.onerror = function(event) {
            console.error('[VoiceControl] Error: ' + event.error);
            if (event.error === 'not-allowed') {
                console.error('[VoiceControl] Microphone permission denied');
            }
        };
        
        window.voiceRecognition.onend = function() {
            console.log('[VoiceControl] Recognition ended, restarting...');
            if (window.voiceRecognitionAutoRestart) {
                window.voiceRecognition.start();
            }
        };
        
        return 1;
    },
    
    VoiceControl_Start: function() {
        if (window.voiceRecognition) {
            window.voiceRecognitionAutoRestart = true;
            window.voiceRecognition.start();
            console.log('[VoiceControl] Started listening');
            return 1;
        }
        return 0;
    },
    
    VoiceControl_Stop: function() {
        if (window.voiceRecognition) {
            window.voiceRecognitionAutoRestart = false;
            window.voiceRecognition.stop();
            console.log('[VoiceControl] Stopped listening');
            return 1;
        }
        return 0;
    },
    
    VoiceControl_IsSupported: function() {
        return ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) ? 1 : 0;
    }
});

#!/usr/bin/env python3
"""
Canicross WebGL Build & Server
Automates WebGL build and starts a local server for mobile testing.
"""

import os
import sys
import subprocess
import time
from pathlib import Path

PROJECT_DIR = Path("/mnt/c/Users/alexa/Projetos/cani-game")
UNITY_PATH = Path("/mnt/c/Program Files/Unity/Hub/Editor/6000.4.5f1/Editor/Unity.exe")
BUILD_DIR = PROJECT_DIR / "Canicross" / "Builds" / "WebGL"

def check_unity_editor_running():
    """Check if Unity Editor is running."""
    try:
        result = subprocess.run(
            ["powershell.exe", "-Command", "Get-Process | Where-Object {$_.ProcessName -like '*Unity*'}"],
            capture_output=True, text=True
        )
        return "Unity" in result.stdout
    except:
        return False

def build_webgl():
    """Run Unity WebGL build."""
    print("Building WebGL...")
    
    cmd = [
        "powershell.exe",
        "-Command",
        f'& "{UNITY_PATH}" -batchmode -nographics '
        f'-projectPath "{PROJECT_DIR / "Canicross"}" '
        f'-executeMethod Canicross.Editor.WebGLBuilder.BuildWebGL '
        f'-logFile "{PROJECT_DIR / "TestResults" / "build_log.txt"}" '
        f'-forgetProjectPath -quit'
    ]
    
    result = subprocess.run(cmd, capture_output=True, text=True)
    
    if result.returncode != 0:
        print(f"Build failed! Check {PROJECT_DIR / 'TestResults' / 'build_log.txt'}")
        return False
    
    print("Build completed successfully!")
    return True

def start_server():
    """Start Node.js web server."""
    print("Starting web server...")
    
    server_script = PROJECT_DIR / "scripts" / "webgl_server.js"
    
    if not server_script.exists():
        print(f"Server script not found: {server_script}")
        return None
    
    process = subprocess.Popen(
        ["node", str(server_script)],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True
    )
    
    time.sleep(2)
    
    if process.poll() is not None:
        stdout, stderr = process.communicate()
        print(f"Server failed to start: {stderr}")
        return None
    
    print("Server running at http://localhost:8080")
    print("To access from mobile:")
    print("  1. Find your computer's IP address: run 'ipconfig' in Windows CMD")
    print("  2. Open http://<YOUR_IP>:8080 on your phone")
    print("  3. Tap the microphone button and speak commands!")
    
    return process

def get_local_ip():
    """Try to get local IP address."""
    try:
        result = subprocess.run(
            ["powershell.exe", "-Command", 
             "(Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.IPAddress -notlike '127.*' -and $_.IPAddress -notlike '169.*'}).IPAddress"],
            capture_output=True, text=True
        )
        ips = [ip.strip() for ip in result.stdout.strip().split('\n') if ip.strip()]
        return ips[0] if ips else None
    except:
        return None

def main():
    print("=" * 60)
    print("CANICROSS - WebGL Build & Mobile Server")
    print("=" * 60)
    
    # Check if Unity Editor is running
    if check_unity_editor_running():
        print("\nWARNING: Unity Editor appears to be running.")
        print("You must close Unity Editor before building WebGL.")
        print("Please close it and run this script again.")
        sys.exit(1)
    
    # Check if build already exists
    if BUILD_DIR.exists() and (BUILD_DIR / "index.html").exists():
        print(f"\nExisting build found at: {BUILD_DIR}")
        response = input("Rebuild? (y/n): ").strip().lower()
        if response == 'y':
            if not build_webgl():
                sys.exit(1)
    else:
        if not build_webgl():
            sys.exit(1)
    
    # Start server
    server_process = start_server()
    
    if server_process:
        local_ip = get_local_ip()
        if local_ip:
            print(f"\nNetwork URL: http://{local_ip}:8080")
        
        print("\nPress Ctrl+C to stop the server.")
        try:
            while True:
                time.sleep(1)
        except KeyboardInterrupt:
            print("\nStopping server...")
            server_process.terminate()
            print("Server stopped.")

if __name__ == "__main__":
    main()

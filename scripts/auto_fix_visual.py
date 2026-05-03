#!/usr/bin/env python3
"""
Auto-fix visual issues in Canicross based on visual test analysis.
Modifies Unity C# scripts to correct common visual problems.
"""

import os
import re
from pathlib import Path

BASE_DIR = Path("/mnt/c/Users/alexa/Projetos/cani-game/Canicross/Assets/_Game/Scripts")

def fix_dog_position():
    """Ensure dog Y position is above ground."""
    wizard_path = BASE_DIR / "Editor" / "GameSetupWizard.cs"
    if not wizard_path.exists():
        print("[AutoFix] GameSetupWizard.cs not found")
        return False
    
    content = wizard_path.read_text()
    
    # Check if dog Y is too low
    if "new Vector3(0, 0.3f, 8f)" in content:
        content = content.replace(
            "new Vector3(0, 0.3f, 8f)",
            "new Vector3(0, 0.6f, 8f)"
        )
        wizard_path.write_text(content)
        print("[AutoFix] Raised dog Y position from 0.3 to 0.6")
        return True
    
    return False


def fix_camera_angle():
    """Improve camera framing if needed."""
    tester_path = BASE_DIR / "Editor" / "VisualTester.cs"
    if not tester_path.exists():
        return False
    
    # Camera angles are already reasonable in VisualTester
    return False


def fix_lighting():
    """Increase ambient light if images are too dark."""
    track_gen_path = BASE_DIR / "Environment" / "TrackGenerator.cs"
    if not track_gen_path.exists():
        return False
    
    content = track_gen_path.read_text()
    
    # Check if ambient light is too dark
    if "new Color(0.6f, 0.65f, 0.75f)" in content:
        content = content.replace(
            "new Color(0.6f, 0.65f, 0.75f)",
            "new Color(0.75f, 0.78f, 0.85f)"
        )
        track_gen_path.write_text(content)
        print("[AutoFix] Increased ambient light brightness")
        return True
    
    return False


def fix_tether_visibility():
    """Make tether more visible."""
    tether_path = BASE_DIR / "Player" / "TetherSystem.cs"
    if not tether_path.exists():
        return False
    
    content = tether_path.read_text()
    modified = False
    
    # Increase rope width
    if "[SerializeField] private float ropeWidth = 0.04f;" in content:
        content = content.replace(
            "[SerializeField] private float ropeWidth = 0.04f;",
            "[SerializeField] private float ropeWidth = 0.06f;"
        )
        modified = True
        print("[AutoFix] Increased tether width for better visibility")
    
    # Brighter color
    if "new Color(0f, 0.75f, 1f, 1f)" in content:
        content = content.replace(
            "new Color(0f, 0.75f, 1f, 1f)",
            "new Color(0.2f, 0.85f, 1f, 1f)"
        )
        modified = True
        print("[AutoFix] Brightened tether color")
    
    if modified:
        tether_path.write_text(content)
    
    return modified


def main():
    print("=" * 50)
    print("CANICROSS - AUTO-FIX VISUAL ISSUES")
    print("=" * 50)
    
    fixes_applied = []
    
    if fix_dog_position():
        fixes_applied.append("Dog position")
    
    if fix_lighting():
        fixes_applied.append("Lighting")
    
    if fix_tether_visibility():
        fixes_applied.append("Tether visibility")
    
    if fixes_applied:
        print(f"\nFixes applied: {', '.join(fixes_applied)}")
        print("Run visual tests again to verify.")
    else:
        print("\nNo automatic fixes were needed or could be applied.")
        print("Manual inspection may be required.")
    
    print("=" * 50)


if __name__ == "__main__":
    main()

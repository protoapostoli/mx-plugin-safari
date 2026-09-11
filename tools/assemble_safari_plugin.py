#!/usr/bin/env python3
"""
assemble_safari_plugin.py - Assembles the final SafariPlugin distribution folder
ready to be linked or copied to LogiPluginService/Plugins/SafariPlugin.
"""

import os
import shutil
from pathlib import Path

BASE_DIR = Path(__file__).resolve().parent.parent
SRC_DIR = BASE_DIR / "plugins" / "safari-controller"
BIN_DEBUG = SRC_DIR / "bin" / "Debug"
DIST_PLUGIN = BASE_DIR / "plugins" / "safari-controller" / "dist" / "Safari"

def assemble():
    print(f"Assembling SafariPlugin into: {DIST_PLUGIN}")
    if DIST_PLUGIN.exists():
        shutil.rmtree(DIST_PLUGIN)
    
    # 1. Create subdirectories
    meta_dir = DIST_PLUGIN / "metadata"
    profiles_dir = DIST_PLUGIN / "profiles"

    meta_dir.mkdir(parents=True, exist_ok=True)
    profiles_dir.mkdir(parents=True, exist_ok=True)

    # 2. Copy binaries to plugin root
    for file in ["SafariPlugin.dll", "SafariPlugin.deps.json", "SafariPlugin.pdb"]:
        src_f = BIN_DEBUG / file
        if src_f.exists():
            shutil.copy2(src_f, DIST_PLUGIN / file)
            print(f"  Copied bin: {file}")

    # 3. Copy metadata
    shutil.copy2(SRC_DIR / "package" / "metadata" / "LoupedeckPackage.yaml", meta_dir / "LoupedeckPackage.yaml")
    icon_src = SRC_DIR / "package" / "metadata" / "Icon256x256.png"
    if icon_src.exists():
        shutil.copy2(icon_src, meta_dir / "Icon256x256.png")
        print("  Copied metadata: Icon256x256.png")
    print("  Copied metadata: LoupedeckPackage.yaml")

    # 4. Copy profiles
    for prof_src in (BASE_DIR / "profiles" / "safari").glob("*.lp5"):
        shutil.copy2(prof_src, profiles_dir / prof_src.name)
        print(f"  Copied profile: {prof_src.name}")

    print("SafariPlugin assembly complete!")

if __name__ == "__main__":
    assemble()

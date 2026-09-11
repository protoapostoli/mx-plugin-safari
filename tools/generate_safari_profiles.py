#!/usr/bin/env python3
"""
generate_safari_profiles.py - Generates complete, valid Loupedeck application profiles (.lp5)
for Safari plugin, targeting both MX Keypad (70/72) and MX Dialpad (71).
"""

import os
import json
import zipfile
from pathlib import Path

BASE_DIR = Path(__file__).resolve().parent.parent

# Stable GUIDs for predictable profile references
GUID_70_PROFILE = "A1B2C3D4E5F640019283746501700001"
GUID_70_WS = "A1B2C3D4E5F640019283746501700002"
GUID_70_PAGE = "A1B2C3D4E5F640019283746501700003"

GUID_71_PROFILE = "B1C2D3E4F5A640019283746501710001"
GUID_71_WS = "B1C2D3E4F5A640019283746501710002"
GUID_71_PRESS_PAGE = "B1C2D3E4F5A640019283746501710003"
GUID_71_ROTATE_PAGE = "B1C2D3E4F5A640019283746501710004"

GUID_72_PROFILE = "C1D2E3F4A5B640019283746501720001"
GUID_72_WS = "C1D2E3F4A5B640019283746501720002"
GUID_72_PAGE = "C1D2E3F4A5B640019283746501720003"

def build_profile_70(out_dir: Path, device_type: str = "Loupedeck70", profile_guid: str = GUID_70_PROFILE, ws_guid: str = GUID_70_WS, page_guid: str = GUID_70_PAGE):
    out_dir.mkdir(parents=True, exist_ok=True)
    meta_dir = out_dir / "metadata"
    meta_dir.mkdir(exist_ok=True)

    # 1. metadata/LoupedeckPackage.yaml
    with open(meta_dir / "LoupedeckPackage.yaml", "w", encoding="utf-8") as f:
        f.write(f"type: Profile5\nname: {profile_guid}\ndisplayName: Default Safari Profile\nversion: 1.0.0\n")

    # 2. ApplicationInfo.json
    app_info = {
        "$type": "Loupedeck.Service.SupportedApplicationInfo, LoupedeckService",
        "name": "@_safari",
        "displayName": "Safari Controller",
        "description": None,
        "deviceType": device_type,
        "nativePluginName": "Safari",
        "hasNativePlugin": True,
        "processOrBundleName": None,
        "modes": [
            {
                "$type": "Loupedeck.Service.ApplicationMode, LoupedeckService",
                "name": "main",
                "parentModeName": None,
                "displayName": "Main"
            }
        ],
        "defaultProfileName": profile_guid,
        "isEnabled": True
    }
    with open(out_dir / "ApplicationInfo.json", "w", encoding="utf-8") as f:
        json.dump(app_info, f, indent=4)

    # 3. ProfileInfo.json
    profile_info = {
        "$type": "Loupedeck.Service.ApplicationProfile, LoupedeckService",
        "name": profile_guid,
        "profileFlags": "None",
        "displayName": "Default Safari Profile",
        "description": "",
        "deviceType": device_type,
        "applicationName": "@_safari",
        "nativePluginName": "Safari",
        "hasNativePlugin": True,
        "additionalNativePluginNames": [
            "Safari",
            "DefaultMac"
        ],
        "lastModifiedTimeUtc": "2026-09-11T12:00:00.000000Z",
        "profileSettings": {
            "$type": "Loupedeck.DictionaryNoCase`1[[System.String, System.Private.CoreLib]], PluginApi"
        },
        "actionImages90": None,
        "actionImages60": None,
        "wheelImages": None,
        "actionColors": None,
        "layout": {
            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayout7, LoupedeckService",
            "deviceType": device_type,
            "profileFlags": "None",
            "layoutModes": [
                {
                    "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutMode7, LoupedeckService",
                    "deviceType": device_type,
                    "modeName": "main",
                    "parentModeName": None,
                    "actions": None,
                    "dynamicButtonPages": None,
                    "dynamicEncoderPages": None,
                    "workspaces": [
                        {
                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutWorkspace7, LoupedeckService",
                            "name": ws_guid,
                            "displayName": "Workspace 1",
                            "description": None,
                            "pressPages": [
                                {
                                    "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutPage7, LoupedeckService",
                                    "name": page_guid,
                                    "displayName": "Page (1)",
                                    "description": None,
                                    "controls": [
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 0,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariNewTabCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 1,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariCloseTabCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 2,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariDuplicateTabCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 3,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariBookmarkletCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 4,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariActiveTabKey",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 5,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariReaderModeCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 6,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariBackCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 7,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariReloadCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 8,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariForwardCommand",
                                            "rotateAction": None
                                        }
                                    ]
                                }
                            ],
                            "rotatePages": []
                        }
                    ],
                    "homeWorkspaceName": ws_guid
                }
            ],
            "folderPages": []
        },
        "macroCommands": [],
        "macroAdjustments": [],
        "profileCommands": [],
        "profileAdjustments": [],
        "conversionHistory": "2026-09-11T12:00:00.0000000Z bj 6.4.1.3246\r\n",
        "packageName": profile_guid,
        "packageVersion": "1.0.0",
        "profileActions": []
    }
    with open(out_dir / "ProfileInfo.json", "w", encoding="utf-8") as f:
        json.dump(profile_info, f, indent=4)

def build_profile_71(out_dir: Path):
    out_dir.mkdir(parents=True, exist_ok=True)
    meta_dir = out_dir / "metadata"
    meta_dir.mkdir(exist_ok=True)

    # 1. metadata/LoupedeckPackage.yaml
    with open(meta_dir / "LoupedeckPackage.yaml", "w", encoding="utf-8") as f:
        f.write(f"type: Profile5\nname: {GUID_71_PROFILE}\ndisplayName: Default Safari Dialpad Profile\nversion: 1.0.0\n")

    # 2. ApplicationInfo.json
    app_info = {
        "$type": "Loupedeck.Service.SupportedApplicationInfo, LoupedeckService",
        "name": "@_safari",
        "displayName": "Safari Controller",
        "description": None,
        "deviceType": "Loupedeck71",
        "nativePluginName": "Safari",
        "hasNativePlugin": True,
        "processOrBundleName": None,
        "modes": [
            {
                "$type": "Loupedeck.Service.ApplicationMode, LoupedeckService",
                "name": "main",
                "parentModeName": None,
                "displayName": "Main"
            }
        ],
        "defaultProfileName": GUID_71_PROFILE,
        "isEnabled": True
    }
    with open(out_dir / "ApplicationInfo.json", "w", encoding="utf-8") as f:
        json.dump(app_info, f, indent=4)

    # 3. ProfileInfo.json
    profile_info = {
        "$type": "Loupedeck.Service.ApplicationProfile, LoupedeckService",
        "name": GUID_71_PROFILE,
        "profileFlags": "None",
        "displayName": "Default Safari Dialpad Profile",
        "description": "",
        "deviceType": "Loupedeck71",
        "applicationName": "@_safari",
        "nativePluginName": "Safari",
        "hasNativePlugin": True,
        "additionalNativePluginNames": [
            "Safari",
            "DefaultMac"
        ],
        "lastModifiedTimeUtc": "2026-09-11T12:00:00.000000Z",
        "profileSettings": {
            "$type": "Loupedeck.DictionaryNoCase`1[[System.String, System.Private.CoreLib]], PluginApi"
        },
        "actionImages90": None,
        "actionImages60": None,
        "wheelImages": None,
        "actionColors": None,
        "layout": {
            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayout7, LoupedeckService",
            "deviceType": "Loupedeck71",
            "profileFlags": "None",
            "layoutModes": [
                {
                    "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutMode7, LoupedeckService",
                    "deviceType": "Loupedeck71",
                    "modeName": "main",
                    "parentModeName": None,
                    "actions": None,
                    "dynamicButtonPages": None,
                    "dynamicEncoderPages": None,
                    "workspaces": [
                        {
                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutWorkspace7, LoupedeckService",
                            "name": GUID_71_WS,
                            "displayName": "Workspace 1",
                            "description": None,
                            "pressPages": [
                                {
                                    "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutPage7, LoupedeckService",
                                    "name": GUID_71_PRESS_PAGE,
                                    "displayName": "Buttons Page",
                                    "description": None,
                                    "controls": [
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 0,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariReopenClosedTabCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 1,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariNewTabCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 2,
                                            "pressAction": "$Safari___Loupedeck.SafariPlugin.Actions.SafariBookmarkletCommand",
                                            "rotateAction": None
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 3,
                                            "pressAction": "$@Generic___Loupedeck.GenericPlugin.ShowRadialMenuDynamicAction",
                                            "rotateAction": None
                                        }
                                    ]
                                }
                            ],
                            "rotatePages": [
                                {
                                    "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutPage7, LoupedeckService",
                                    "name": GUID_71_ROTATE_PAGE,
                                    "displayName": "Rotary Controls",
                                    "description": None,
                                    "controls": [
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 0,
                                            "pressAction": None,
                                            "rotateAction": "$Safari___Loupedeck.SafariPlugin.Adjustments.SafariScrollAdjustment"
                                        },
                                        {
                                            "$type": "Loupedeck.Service.Devices.Loupedeck7Devices.ProfileLayoutControl7, LoupedeckService",
                                            "controlId": 1,
                                            "pressAction": None,
                                            "rotateAction": "$Safari___Loupedeck.SafariPlugin.Adjustments.SafariTabScrubAdjustment"
                                        }
                                    ]
                                }
                            ]
                        }
                    ],
                    "homeWorkspaceName": GUID_71_WS
                }
            ],
            "folderPages": []
        },
        "macroCommands": [],
        "macroAdjustments": [],
        "profileCommands": [],
        "profileAdjustments": [],
        "conversionHistory": "2026-09-11T12:00:00.0000000Z bj 6.4.1.3246\r\n",
        "packageName": GUID_71_PROFILE,
        "packageVersion": "1.0.0",
        "profileActions": []
    }
    with open(out_dir / "ProfileInfo.json", "w", encoding="utf-8") as f:
        json.dump(profile_info, f, indent=4)

def zip_directory(in_dir: Path, out_zip: Path):
    out_zip.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(out_zip, "w", compression=zipfile.ZIP_DEFLATED) as zf:
        for root, _, files in os.walk(in_dir):
            for file in files:
                full_path = Path(root) / file
                rel_path = full_path.relative_to(in_dir)
                zf.write(full_path, arcname=str(rel_path))
    print(f"Created: {out_zip} ({out_zip.stat().st_size} bytes)")

def main():
    tmp_70 = BASE_DIR / "scratch" / "DefaultProfile70"
    tmp_71 = BASE_DIR / "scratch" / "DefaultProfile71"
    tmp_72 = BASE_DIR / "scratch" / "DefaultProfile72"

    build_profile_70(tmp_70, "Loupedeck70", GUID_70_PROFILE, GUID_70_WS, GUID_70_PAGE)
    build_profile_71(tmp_71)
    build_profile_70(tmp_72, "Loupedeck72", GUID_72_PROFILE, GUID_72_WS, GUID_72_PAGE)

    dest_dirs = [
        BASE_DIR / "src" / "package" / "profiles",
    ]

    for d in dest_dirs:
        # Generate both Mac-suffixed and non-suffixed versions for maximum compatibility
        zip_directory(tmp_70, d / "DefaultProfile70Mac.lp5")
        zip_directory(tmp_70, d / "DefaultProfile70.lp5")
        zip_directory(tmp_71, d / "DefaultProfile71Mac.lp5")
        zip_directory(tmp_71, d / "DefaultProfile71.lp5")
        zip_directory(tmp_72, d / "DefaultProfile72Mac.lp5")
        zip_directory(tmp_72, d / "DefaultProfile72.lp5")

    print("Safari profiles successfully built and synchronized!")

if __name__ == "__main__":
    main()

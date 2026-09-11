#!/usr/bin/env python3
"""
profile_tool.py - Utility to inspect, extract, build, and analyze Logitech MX profiles (.lp5).

Supported devices:
  - Loupedeck70: MX Creative Keypad (9 LCD keys, 2 paging buttons)
  - Loupedeck71: MX Creative Dialpad (Dial, Roller, 4 buttons)
  - Loupedeck72: Actions Ring / Combined
"""

import sys
import os
import json
import zipfile
import argparse
from pathlib import Path

LOGI_DIR = Path.home() / "Library/Application Support/Logi/LogiPluginService"
APPLICATIONS_DIR = LOGI_DIR / "Applications"

DEVICE_NAMES = {
    "Loupedeck70": "MX Creative Keypad (9 LCD buttons)",
    "Loupedeck71": "MX Creative Dialpad (Dial, Roller, 4 buttons)",
    "Loupedeck72": "Actions Ring / Universal Overlay",
    "Loupedeck20": "Loupedeck CT",
    "Loupedeck30": "Loupedeck Live",
    "Loupedeck50": "Loupedeck Live S",
}

def inspect_profile_data(profile_info: dict, source_name: str):
    device_type = profile_info.get("deviceType", "Unknown")
    display_name = profile_info.get("displayName", "Unnamed")
    app_name = profile_info.get("applicationName", "General")
    native_plugin = profile_info.get("nativePluginName", "None")

    print(f"\n========================================================")
    print(f"Profile:       {display_name}")
    print(f"Source:        {source_name}")
    print(f"Device:        {device_type} ({DEVICE_NAMES.get(device_type, 'Custom/Other')})")
    print(f"Application:   {app_name}")
    print(f"Native Plugin: {native_plugin}")
    print(f"========================================================")

    layout = profile_info.get("layout", {})
    layout_modes = layout.get("layoutModes", [])

    for mode in layout_modes:
        mode_name = mode.get("modeName", "Default")
        workspaces = mode.get("workspaces", [])
        print(f"\n[Mode: {mode_name}] (Workspaces: {len(workspaces)})")

        for ws in workspaces:
            ws_name = ws.get("displayName", ws.get("name", "Workspace"))
            print(f"\n  Workspace: {ws_name}")

            press_pages = ws.get("pressPages", [])
            for p_idx, page in enumerate(press_pages):
                page_name = page.get("displayName", f"Page {p_idx + 1}")
                print(f"    Press Page: {page_name}")
                for ctrl in page.get("controls", []):
                    cid = ctrl.get("controlId")
                    press_act = ctrl.get("pressAction") or "None"
                    print(f"      Control ID {cid:2d} -> Press: {press_act}")

            rotate_pages = ws.get("rotatePages", [])
            for r_idx, page in enumerate(rotate_pages):
                page_name = page.get("displayName", f"Rotate Page {r_idx + 1}")
                print(f"    Rotate Page: {page_name}")
                for ctrl in page.get("controls", []):
                    cid = ctrl.get("controlId")
                    rot_act = ctrl.get("rotateAction") or "None"
                    if cid == 0:
                        ctrl_label = "Dial (0)"
                    elif cid == 1:
                        ctrl_label = "Roller (1)"
                    else:
                        ctrl_label = f"Control ({cid})"
                    print(f"      {ctrl_label:12s} -> Rotate: {rot_act}")

def inspect_file(path_str: str):
    path = Path(path_str)
    if not path.exists():
        print(f"Error: Path '{path_str}' does not exist.", file=sys.stderr)
        sys.exit(1)

    if path.is_dir():
        # Look for ProfileInfo.json
        info_file = path / "ProfileInfo.json"
        if not info_file.exists():
            print(f"Error: {info_file} not found in directory.", file=sys.stderr)
            sys.exit(1)
        with open(info_file, "r", encoding="utf-8") as f:
            data = json.load(f)
        inspect_profile_data(data, str(path))

    elif zipfile.is_zipfile(path):
        with zipfile.ZipFile(path, "r") as zf:
            if "ProfileInfo.json" not in zf.namelist():
                print(f"Error: ProfileInfo.json not found inside {path}", file=sys.stderr)
                sys.exit(1)
            with zf.open("ProfileInfo.json") as f:
                data = json.load(f)
            inspect_profile_data(data, str(path))
    else:
        print(f"Error: '{path_str}' is neither a directory nor a zip (.lp5) file.", file=sys.stderr)
        sys.exit(1)

def extract_profile(lp5_path_str: str, out_dir_str: str):
    lp5_path = Path(lp5_path_str)
    out_dir = Path(out_dir_str)
    if not lp5_path.is_file() or not zipfile.is_zipfile(lp5_path):
        print(f"Error: '{lp5_path}' is not a valid .lp5 zip file.", file=sys.stderr)
        sys.exit(1)

    out_dir.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(lp5_path, "r") as zf:
        zf.extractall(out_dir)

    # Prettify any json files in extracted dir for easy editing
    for root, _, files in os.walk(out_dir):
        for f in files:
            if f.endswith(".json"):
                full_f = Path(root) / f
                try:
                    with open(full_f, "r", encoding="utf-8") as jf:
                        jdata = json.load(jf)
                    with open(full_f, "w", encoding="utf-8") as jf:
                        json.dump(jdata, jf, indent=2)
                except Exception:
                    pass

    print(f"Successfully extracted '{lp5_path.name}' to: {out_dir}")

def pack_profile(in_dir_str: str, out_lp5_str: str):
    in_dir = Path(in_dir_str)
    out_lp5 = Path(out_lp5_str)
    if not in_dir.is_dir():
        print(f"Error: '{in_dir}' is not a directory.", file=sys.stderr)
        sys.exit(1)

    if not (in_dir / "ProfileInfo.json").exists():
        print(f"Warning: '{in_dir / 'ProfileInfo.json'}' does not exist! Are you sure this is a valid profile folder?")

    out_lp5.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(out_lp5, "w", compression=zipfile.ZIP_DEFLATED) as zf:
        for root, _, files in os.walk(in_dir):
            for file in files:
                full_path = Path(root) / file
                rel_path = full_path.relative_to(in_dir)
                zf.write(full_path, arcname=str(rel_path))

    print(f"Successfully packed '{in_dir}' into '{out_lp5}' ({out_lp5.stat().st_size} bytes)")

def list_active_profiles():
    print("\nActive Device Applications and Profiles:")
    if not APPLICATIONS_DIR.exists():
        print(f"Applications directory {APPLICATIONS_DIR} not found.")
        return

    for dev in sorted(os.listdir(APPLICATIONS_DIR)):
        dev_path = APPLICATIONS_DIR / dev
        if not dev_path.is_dir():
            continue
        dev_label = DEVICE_NAMES.get(dev, dev)
        print(f"\n--- {dev} ({dev_label}) ---")
        for app in sorted(os.listdir(dev_path)):
            app_path = dev_path / app
            if not app_path.is_dir():
                continue
            profiles_dir = app_path / "Profiles"
            p_count = 0
            if profiles_dir.exists():
                p_count = len([p for p in os.listdir(profiles_dir) if (profiles_dir / p).is_dir()])
            print(f"  App: {app:35s} (Profiles: {p_count})")

def main():
    parser = argparse.ArgumentParser(description="Logitech MX Creative Console Profile Tool")
    subparsers = parser.add_subparsers(dest="subcommand", help="Subcommand to run")

    # inspect
    p_inspect = subparsers.add_parser("inspect", help="Inspect a profile (.lp5 file or directory)")
    p_inspect.add_argument("path", help="Path to .lp5 file or profile folder containing ProfileInfo.json")

    # extract
    p_extract = subparsers.add_parser("extract", help="Extract an .lp5 archive to a folder with formatted JSON")
    p_extract.add_argument("lp5_path", help="Path to input .lp5 file")
    p_extract.add_argument("output_dir", help="Destination folder path")

    # pack
    p_pack = subparsers.add_parser("pack", help="Pack a folder into an .lp5 archive")
    p_pack.add_argument("input_dir", help="Path to profile folder")
    p_pack.add_argument("output_lp5", help="Path to output .lp5 file")

    # list
    subparsers.add_parser("list", help="List all applications and profiles currently configured on this Mac")

    args = parser.parse_args()

    if args.subcommand == "inspect":
        inspect_file(args.path)
    elif args.subcommand == "extract":
        extract_profile(args.lp5_path, args.output_dir)
    elif args.subcommand == "pack":
        pack_profile(args.input_dir, args.output_lp5)
    elif args.subcommand == "list" or not args.subcommand:
        list_active_profiles()

if __name__ == "__main__":
    main()

# A script for single [double] click uploading to itch using butler
# by Nothke
#
# Requirements:
# - Installed butler: https://itch.io/docs/butler/
# - butler added to PATH
#
# How to use:
# 1. Put this script in your project folder,
# 2. Edit the script by adding project names and ignores below
# 3. Run the script!

import os
import subprocess
import shutil
from shutil import ignore_patterns

# --------------------------------------------
# ---- Add your project's specifics here! ----
# --------------------------------------------
source = "Build"                    # folder relative to script
target = "nothke/quality-control"   # target itch project user/game
channel = "windows"                 # win|windows/linux/mac|osx/android [-stable, -beta]

# Add subfolders or patterns here to ignore
ignores = [
    "dontuploadthis",
    "MyIL2CPPGameName_BackUpThisFolder_ButDontShipItWithYourGame"
    ]

# XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
# XXXXXX Code, don't touch this: XXXXXXX

script_path = os.path.dirname(os.path.realpath(__file__))
source_path = script_path + "\\" + source

if not os.path.isdir(source_path):
    print("Source folder " + source_path + " doesn't exist or is not a folder")
    os._exit(1)

input("Ready to publish. Are you sure? Hit enter..")

# Copy to temporary folder to ignore certain files
#os.mkdir(script_path + "\\" + "butler_script_temp")
temp_path = script_path + "\\butler_script_temp"
shutil.copytree(source_path, temp_path,
    ignore = ignore_patterns(*ignores))

#print("calling: butler push " + source_path + " " + target + ":" + channel)

# UPLOAD TO ITCH!
subprocess.call(['butler', 'push', temp_path, target + ":" + channel])

#remove the temporary folder
shutil.rmtree(temp_path)

input("Press Enter to close..")
# -- PROMPT -----------------------------------------------------------------------------------------------------------
# Please write a Python 3 application that will organize the files on my desktop into subfolders based on file 
# extensions such as images, documents, and zip archives.
# ---------------------------------------------------------------------------------------------------------------------
quit()  # NOTE: this is here because I honestly do not want to run this, I'm just following along with the video
# ---------------------------------------------------------------------------------------------------------------------

from pathlib import Path
import shutil

def organize_desktop():
    # Get desktop path (Windows)
    desktop = Path.home() / "Desktop"
    
    # Define extension categories
    categories = { "Images": [".jpg", ".jpeg", ".png", ".gif", ".bmp"]
                  ,"Documents": [".txt", ".doc", ".docx", ".pdf", ".csv"]
                  ,"Zip_Archives": [".zip", ".rar", ".7z"]  }
    
    # Create subfolders if they don't exist
    for category in categories:
        folder_path = desktop / category
        folder_path.mkdir(exist_ok=True)
    
    # Iterate through desktop files
    for file_path in desktop.iterdir():
        if file_path.is_file():  # Process only files
            extension = file_path.suffix.lower()
            for category, extensions in categories.items():
                if extension in extensions:
                    try:
                        destination = desktop / category / file_path.name
                        shutil.move(str(file_path), str(destination))
                        print(f"Moved {file_path.name} to {category}")
                    except Exception as e:
                        print(f"Error moving {file_path.name}: {type(e).__name__} - {e}")
                    break
            else:
                print(f"Skipped {file_path.name}: Unknown extension {extension}")

if __name__ == "__main__":
    print(f"Organizing files on {Path.home() / 'Desktop'}")
    organize_desktop()
    print("Organization complete!")
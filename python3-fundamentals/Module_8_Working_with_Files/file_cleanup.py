# ---------------------------------------------------------------------------------------------------------------------
# import(s)
# ---------------------------------------------------------------------------------------------------------------------
import os     # NOTE: 'os' interacts with the Operating System, some features are: make directories, list files, and 
              #       move files 

# ---------------------------------------------------------------------------------------------------------------------
# 'os' Function(s)
# ---------------------------------------------------------------------------------------------------------------------
repo_folder = 'D:\\repos\\randel-bjorkquist\\pluralsight\\pluralsight-python3-fundamentals\\module_8_working_with_files'


# NOTE: I honestly do not want to make folders, just because ...
#os.mkdir(temp_folder) # NOTE: creates the folder ... DUH


entries = os.scandir(repo_folder) # NOTE: 'scandir' retrieves all the entries (files/folders) of a directory
for entry in entries:
  if os.path.isfile(entry):
    print('File:', entry.name)
  elif os.path.isdir(entry):
    print('Directory:', entry.name)


file_name = 'new.txt'
new_file_path = os.path.join(repo_folder, file_name)
print(new_file_path)


source_folder       = 'C:\\temp'
destination_folder  = 'D:\\temp'

source_file_path      = os.path.join(source_folder      ,'test.txt')
destination_file_path = os.path.join(destination_folder ,'test.txt')

# NOTE: I honestly do not want to move files, just because ...
#os.rename(source_file_path, destination_file_path)  # NOTE: os.rename() allows us not only to move a file to a new path
#                                                    #       but it also allows us to rename it at the same time.

# ---------------------------------------------------------------------------------------------------------------------
# Program Step(s)
# ---------------------------------------------------------------------------------------------------------------------
#1. Make the folder CleanUp/
#2. List the files in the Desktop/ folder
#3. For each file in the Desktop/ folder
#   a. Move the file to the CleanUp folder

#folder_source       = 'C:/temp'
#folder_destination  = 'D:/temp/pluralsight/python3_fundamentals/module8_working_with_files'

folder_source       = 'D:\\JetBrains'
folder_destination  = os.path.join(folder_source, 'CleanUp')

#os.mkdir(folder_destination)

# NOTE: this is not needed, see additional note below ...
# entries = os.scandir(folder_source)
for entry in os.scandir(folder_source):   # NOTE: shortcut, don't need to hold the list of folder entities in a variable
  source_file_path      = os.path.join(folder_source      ,entry.name)
  destination_file_path = os.path.join(folder_destination ,entry.name)  

  # NOTE: At this time, we DO NOT want to move directories
  if os.path.isfile(entry):
    print(f"Moving '{entry.name}' from '{source_folder}' to '{destination_folder}'")
    #os.rename(source_file_path, destination_file_path)



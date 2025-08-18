# 1: ask the user what acronym they want to look up?
# 2: open the file
# 3: Read each line of the file until the user acronym is found
#   a: Check if the current acronym matches the user's acronym
#   b: IF it matches, Print the definition

# ---------------------------------------------------------------------------------------------------------------------
# import(s)
# ---------------------------------------------------------------------------------------------------------------------
from pathlib import Path

# ---------------------------------------------------------------------------------------------------------------------
current_directory = str(Path.cwd())
current_module    = 'module_8_working_with_files'
file_name         = 'software_acronyms.txt'
file_path         = current_directory + '\\' + current_module + '\\' + file_name

# NOTE: The 'with' keyword makes sure the File is property closed when the file operations are done, eevn if an
# Option 1: a shorter way to close a file ...
#       exception is raised.
#with open(temp_file) as file:
#with open("module_8_working_with_files/software_acronyms.txt") as file:
with open(file_path) as file:
  # Do file operations here ...
  pass

# ---------------------------------------------------------------------------------------------------------------------
# Option 2: a longer way to close a File without the keyword 'with'
file = open(file_path) # QUESTION: I'm not sure why this is outside the try-finally block, I would expect 
                       #           it inside. But, this is where she put it, so this is where I have it.                            
try:
  # do file operations here
  pass  # here only because I don't have any file operations defined
finally:
  file.close()

# ---------------------------------------------------------------------------------------------------------------------
# NOTE: 'read()' - simply returns the whole file as a String by default; or it will return the specified 
#       number of bytes.
print()
print('Prints the entire file as a String, all at once ...')

with open(file_path) as file:
  result = file.read()
                        
  print(result)

# ---------------------------------------------------------------------------------------------------------------------
# NOTE: 'readline()' - returns the 'next' line in the file as a String by default
print()
print('Prints the first 2 file of the file ...')

with open(file_path) as file:
  result = file.readline()  # OUTPUT: 'IDE - Integrated Development Environment'
  print(result)

  result = file.readline()  # OUTPUT: 'OOP - Object Oriented Programming'
  print(result)

# ---------------------------------------------------------------------------------------------------------------------
# NOTE: 'readlines()' - returns a list of Strings of all the lines in the file
print()
print('Printing the entire file, one line at a time ...')

with open(file_path) as file:
  lines = file.readlines()

  for line in lines:
    print(line) # NOTE: this includes the newline character

# ---------------------------------------------------------------------------------------------------------------------
# NOTE: Alternative to 'readlines()', still returns a list of Strings of all the lines in the file
print()
print('Printing the entire file, alternative to readlines(), one line at a time ...')

with open(file_path) as file: # NOTE: since this type of loop is used so often, this is a shortcut, directly looping over
  for line in file:           #       the File Object.
    print(line, end ='')      # NOTE: this removes the newline character

# ---------------------------------------------------------------------------------------------------------------------


# ---------------------------------------------------------------------------------------------------------------------

# ---------------------------------------------------------------------------------------------------------------------

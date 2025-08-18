# ---------------------------------------------------------------------------------------------------------------------
# import(s)
# ---------------------------------------------------------------------------------------------------------------------
from pathlib import Path

# ---------------------------------------------------------------------------------------------------------------------
# Global Variable(s)
# ---------------------------------------------------------------------------------------------------------------------
current_directory = str(Path.cwd())
current_module    = 'module_8_working_with_files'
file_name         = 'software_acronyms.txt'
file_path         = current_directory + '\\' + current_module + '\\' + file_name

YES_NO   = {'y': True, 'yes': True, 'n': False, 'no': False}
FIND_ADD = {'F': 'F', 'A': 'A'}
#FIND_ADD = [ (lambda v: v in ('F', 'f'), 'F')
#            ,(lambda v: v in ('A', 'a'), 'A') ]

# ---------------------------------------------------------------------------------------------------------------------
# Function(s)
# ---------------------------------------------------------------------------------------------------------------------
"""
A flexible input helper.
- choices: dict of normalized input -> return value (e.g., {'y': True, 'yes': True})
- parser: callable to convert input (e.g., int, float, lambda s: s.lower())
- validate: callable(value) -> bool (e.g., lambda x: 90 <= x <= 110)
- default: value returned if user submits empty input
- normalize: callable to preprocess raw text (default: strip)
"""
def ask(prompt, *, parser=None, choices=None, validate=None,
        default=None, normalize=str.strip, retry_msg=None):
  
  while True:
    raw = input(prompt)
    if raw == "" and default is not None:
      return default
    
    text = normalize(raw) if normalize else raw
    
    if choices is not None:
      if text in choices:
        return choices[text]
      print(retry_msg or "Please enter one of: " + ", ".join(choices.keys()))
      continue
    
    if parser is not None:
      try:
        val = parser(text)
      except Exception:
        print(retry_msg or "Invalid input. Please try again.")
        continue
      
      if validate and not validate(val):
        print(retry_msg or "Value not in accepted range. Please try again.")
        continue
      return val
    
    print(retry_msg or "Please enter a valid value.")

# ---------------------------------------------------------
def add_acronum() -> None:
  acronym    = input("What acronym would you like to add? ")
  definition = input("What is the acronym's definition? ")

  with open(file_path, 'a') as file:
    file.write(acronym + ' - ' + definition + '\n')

# ---------------------------------------------------------
def find_acronym() -> None:
  found   = False
  acronym = input("What software acronym would you like to look up? ")

  try:
    with open(file_path) as file:
      for line in file:
        if acronym in line:        
          found = True
          print(line)
          break
  except FileNotFoundError as ex:
    print(f"EXCEPTION: File '{file_name}' not found")
    return

  if not found:
    print(f"The acronym you requested '{acronym}', does not exist.")

# ---------------------------------------------------------
def main():
  # ask the user whether they want to find or add an acronym
  #choice = str(input('Do you want to fine(F) or add(A) an acronym?')).upper()
  choice = ask( 'Do you want to fine(F) or add(A) an acronym? '
               ,parser    = str
               ,choices   = FIND_ADD
               ,normalize = lambda s: s.strip().upper())

  if choice == 'F':
    find_acronym()
  elif choice == 'A':
    add_acronum()
  else:
    Exception("EXCEPTION: Since I'm using the 'ask(...)' function, the code should never get here!")

  return ask( prompt = "Again? (y/n): "
              ,choices = YES_NO
              ,normalize = lambda s: s.strip().lower())

# ---------------------------------------------------------------------------------------------------------------------
# Main Program
# ---------------------------------------------------------------------------------------------------------------------
while True:
  if not main():
    break

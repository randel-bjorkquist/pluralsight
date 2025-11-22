# ---------------------------------------------------------------------------------------------------------------------
# NOTE: initializing, with base values
# acronyms = []
acronyms = ['LOL', 'IDK', 'SMH', 'TBH']

acronyms.append('BFN')
acronyms.append('IMHO')

#print(acronyms[0])
print(acronyms)

acronyms.remove('BFN')  # NOTE: use when you know the object you want to remove
del acronyms[4]         # NOTE: use when you don't know the object you want to remove, but you do know its index

print(acronyms)

# ---------------------------------------------------------------------------------------------------------------------
numbers = [1, 2, 3, 4, 5]

if 1 in numbers:
  print(f"{1} already exists in the 'numbers' list, it's at index {numbers.index(1)}.")

# ---------------------------------------------------------------------------------------------------------------------
word = 'BFN'
#word = 'LOL'

if word in acronyms:
  print(f"Item Found: '{word}' is already in the acronyms list; it's at index {acronyms.index(word)}.")
else:
  print(f"Item Not Found: it's okay to try and add '{word}', as it's not already in the acronyms list.")

print(f"'{word}' {"is" if word in acronyms else "is not"} in the list.")

# ---------------------------------------------------------------------------------------------------------------------
for acronym in acronyms:
  print(acronym)



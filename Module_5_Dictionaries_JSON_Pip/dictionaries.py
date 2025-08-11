# ----------------------------------------------------------------------------------------------------------------------
acronyms      = ['LOL'            ,'IDK'          ,'TBH'         ]
translations  = ['Laugh Out Load' ,"I Don't Know" ,'To Be Honest']

print(acronyms)
print(translations)

del acronyms[0]       # deleted 'LOL'
del translations[0]   # deletes 'Laugh Out Load'

print(acronyms)
print(translations)

# ----------------------------------------------------------------------------------------------------------------------
print()

# ----------------------------------------------------------------------------------------------------------------------
# EXAMPLE: Dictionary of strings-to-strings
#            KEYs | VALUEs
acronyms = { "LOL": "Laugh Out Load"
            ,"IDK": "I Don't Know"
            ,"TBH": "To Be Honest" }

print(acronyms)
print()

print(f"{'LOL'} = {acronyms['LOL']}")  # outputs: "Laugh Out Load"
print(f"{'IDK'} = {acronyms["IDK"]}")  # outputs: "I Don't Know"
print(f"{'TBH'} = {acronyms["TBH"]}")  # outputs: "To Be Honest"

# ----------------------------------------------------------------------------------------------------------------------
# EXAMPLE: Dictionary of strings-to-numbers
menu = { "Soup" : 5
        ,"Salad": 6 }

# EXAMPLE: Dictionary of anything
my_dictionary = { 10: 'hello'
                 , 2: 6.5 }

# ----------------------------------------------------------------------------------------------------------------------
acronyms = {} # creates an empty dictionary

acronyms['LOL'] = "Laugh Out Load"  # addes "Laugh Out Load" to the dictionary
acronyms['IDK'] = "I Don't Know"    # addes "I Don't Know" to the dictionary
acronyms['TBH'] = "To Be Honest"    # addes "To Be Honest" to the dictionary

# ----------------------------------------------------------------------------------------------------------------------
print()

# ----------------------------------------------------------------------------------------------------------------------
acronyms = { "LOL": "Laugh Out Load"
            ,"IDK": "I Don't Know"
            ,"TBH": "To Be Honest" }

print(f"{'TBH'} = {acronyms["TBH"]}")  # outputs: "TBH = To Be Honest"

acronyms['TBH'] = "Honestly"           # updates the value, setting it to "Honestly"

print(f"{'TBH'} = {acronyms["TBH"]}")  # outputs: "TBH = Honestly"

# ----------------------------------------------------------------------------------------------------------------------
print()

# ----------------------------------------------------------------------------------------------------------------------
print(acronyms)

del acronyms['IDK']   # deletes the dictionary entry for the key 'LOL'

print(acronyms)

# ----------------------------------------------------------------------------------------------------------------------
print()

# ----------------------------------------------------------------------------------------------------------------------
# NOTE: 'None' Type
#       - None means the absence of a value, and values to False in a conditional.
#       - It's kind of like 'null' (C, C++, and C#) if it evaluated to false in conditional statements, but it still
#         representds the absence of a value.

#definition = acronyms['BTW']  # throws a "KeyError" => "KeyError: 'BTW'"

# NOTE: to avoid KeyErrors, use the 'get(key_name)' method; it returns the 'None' type if the key is not found.
definition = acronyms.get('BTW')

print(definition)   # outputs: None

# How it's used ...
if definition:        # False because definition is of the None type
  print(definition)
else:
  print("INFO: Key does not exists!")

# ----------------------------------------------------------------------------------------------------------------------
print()

# ----------------------------------------------------------------------------------------------------------------------
acronyms = { "LOL": "Laugh Out Load"
            ,"IDK": "I Don't Know"
            ,"TBH": "To Be Honest" }

sentence    = 'IDK' + ' what happened ' + 'TBH'
translation = acronyms.get('IDK') + ' what happened ' + acronyms.get('TBH')

print('   sentence:', sentence)
print('translation:', translation)


# ----------------------------------------------------------------------------------------------------------------------

# ----------------------------------------------------------------------------------------------------------------------

# ----------------------------------------------------------------------------------------------------------------------

# ----------------------------------------------------------------------------------------------------------------------

# ----------------------------------------------------------------------------------------------------------------------

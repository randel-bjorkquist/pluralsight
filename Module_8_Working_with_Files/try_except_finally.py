# ---------------------------------------------------------------------------------------------------------------------
acronyms = { 'LOL': "laugh out loud"
            ,'IDK': "I don't know"
            ,'TBH': "to be honest" }

try:
  acronym    = 'BTW'
  definition = acronyms[acronym]  
  
  print('Definition of ', acronym, ' is ', definition)

except:
  print(f"The key '{acronym}' does not exist")

finally:
  print('The acroymns we have defined are:')

  for acronym in acronyms:
    print(acronym)

print('The program keeps going ...')



# ---------------------------------------------------------

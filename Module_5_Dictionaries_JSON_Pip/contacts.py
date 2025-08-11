contacts = { 'number': 4
            ,'students': [ {'name': 'Sarah Holderness' ,'email': 'sarah@example.com'}
                          ,{'name': 'Harry Potter'     ,'email': 'harry.potter@example.com'}
                          ,{'name': 'Hermione Granger' ,'email': 'hermione.granger@example.com'}
                          ,{'name': 'Ronald Weasley'   ,'email': 'ronald.weasley@example.com'} ]}

students = contacts['students']

print('Student emails:')

for student in students:
  print('  ', student['email'])

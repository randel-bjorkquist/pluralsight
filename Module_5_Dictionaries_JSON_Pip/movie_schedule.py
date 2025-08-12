# ----------------------------------------------------------------------------------------------------------------------
movies = { 'The Grinch': '11:00 AM'
          ,'Rudolph': '1:00 PM'
          ,'Frosty The Snowman': '3:00 PM'
          ,'Christmas Vacation': '5:00 PM' }

print("We're currently showing the following movies:")

for key in movies:
  print(key)

movie = input("Which movie would you like to see the showtime for? ").title()

showtime = movies.get(movie)

if showtime == None:
  print(f"The requested move '{movie}', is not playing.")
else:
  print(f"{movie} is playing at: {showtime}")


# ----------------------------------------------------------------------------------------------------------------------

# ---------------------------------------------------------------------------------------------------------------------
expenses = [10.50, 8, 5, 15, 20, 5, 3]
total = sum(expenses)

print('You spent $', total, sep = '')
print()

# ---------------------------------------------------------------------------------------------------------------------
sum = 0

for expense in expenses:
  sum += expense

print(sum)
print('You spent $', sum, sep = '')
print(f'You spent ${sum:.2f}')

# ---------------------------------------------------------------------------------------------------------------------

# NOTE: the 'range' function, returns a range of numbers, from the starting value to the ending value, by a step interval
# range(7)        = [0, 1, 2, 3, 4, 5, 6]
# range(0, 7, 1)  = [0, 1, 2, 3, 4, 5, 6]
# range(0, 7, 2)  = [0, 2, 4, 6]
# range(2, 14, 2) = [2, 4, 6, 8, 10, 12]

numbers = list(range(0, 7, 2))
print(numbers)

for i in range(7):
  print(f"i is '{i}'")

# ---------------------------------------------------------------------------------------------------------------------
number_of_expenses = int(input("Enter the number of expenses you which to enter: "))
new_total    = 0
new_expenses = []

for i in range(number_of_expenses):
  new_expenses.append(float(input(f"({i + 1} of {number_of_expenses}) Enter an expense: ")))

for expense in new_expenses:
  new_total += expense

print('You spent $', new_total, sep = '')
print(f'You spent ${new_total:.2f}')

# ---------------------------------------------------------------------------------------------------------------------

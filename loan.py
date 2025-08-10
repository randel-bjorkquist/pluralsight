# Get details of loan
money_owed  = float(input('In dollars, how much money do you own? '))             # example: $50,000
apr         = float(input('What is the annual percentage rate of the loan? '))    # example:     6.5%
payment     = float(input('In dollars, how much will you pay off each month? '))  # example: $ 1,000
months      = int(input('How many months do you want to see the results for? '))  # example:      24

monthly_rate = apr / 100 / 12   # NOTE: we divide apr by 100 to turn it into a yearly percetage and
                                #       divide that by 12 to get the monthly_rate.
month = 0

# NOTE: I did this on my own and didn't stop after 'x' number of months. I went till the load was 
#       paid off.  I did that with the 'while-loop'.
#while int(money_owed) > 0:
#  month += 1

# NOTE: the instrutor did a for-loop and stopping the calculations, regardless if the loan was
#       paid off or not.
for month in range(months):
  # Calculate the interest for the first month and add that to the amount we owe
  interest_2_pay = money_owed * monthly_rate

  # Add interest to pay to money_owed
  money_owed += interest_2_pay

  # Adjust the last payment to zero out the account
  if money_owed < payment:
     payment = money_owed

  # Apply payment
  money_owed -= payment

  # Output calculations:
  #print(f"Here's how your payment was applied:")
  #print(f"    monthly interest amount ({apr:.1f}%): ${interest_2_pay:.2f}")
  #print(f"            paid towards principle: ${(payment - interest_2_pay):.2f}")
  #print(f"                 remaining balance: ${money_owed:.2f}")

  print(f"(Payment {month} of {months}): we have applied your ${payment:.2f} payment, of which ${interest_2_pay:.2f} was interest;", end = ' ')
  print(f"you now owe ${money_owed:.2f}.")

  # NOTE: if the load gets paid off before the length of the loan, let them know and exit the loop ...
  if money_owed == 0:
    print("Congradulations, your loan has succesfully been paid off :-)")
    break

# NOTE: We've cycled through the requested length of the load; if there's still a balance, let them know ...
if money_owed > 0:
  print("The length of the loan is not long enough to pay it off; you'll have to extend the lenght.")

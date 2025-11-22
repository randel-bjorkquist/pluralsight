# ---------------------------------------------------------------------------------------------------------------------
# import(s)
# ---------------------------------------------------------------------------------------------------------------------

# ---------------------------------------------------------------------------------------------------------------------
# Function(s)
# ---------------------------------------------------------------------------------------------------------------------
def remainder_division(a, b) -> None:
  if b == 0:
    raise ZeroDivisionError
    #raise ZeroDivisionError('this really stinks')  # NOTE: can add a custom message to the built-in exception type
    #raise Exception('Divisor cannot be 0')
    #raise InvalidArguementException()  # NOTE: not a real exception type, would have to build a custom exception type
  
  result    = a // b
  remainder = a  % b

  print(a, '/', b, 'is', result, 'remainder', remainder)

# ---------------------------------------------------------------------------------------------------------------------
# Main Program
# ---------------------------------------------------------------------------------------------------------------------
try:
  remainder_division(10, 3)
  remainder_division(10, 0)
except Exception as ex:
  exception_type = type(ex).__name__  # Get exception type name (e.g., "ValueError")
  print(f"Caught {exception_type}: {ex}")
  #print(f"The following exception was thrown: {ex}")


## Functional Requirements

The following list represents all the targeted functional requirements based on the role of each user: **Guest**, **Member (Registered User)**, and **Admin**.

### A. Book Exploration & Details (Guest, Member, Admin)

1. All users shall be able to view the home page.
2. All users shall be able to view the detailed book page (including pricing, book status (available for purchase or lend), and due date if the book is borrowed).
3. All users shall be able to search for books using criteria such as title, author, or category.
4. All users shall be able to filter search results based on availability (available for lending or borrowing) or type of book (for lending or sale).

### B. Authentication & Account Management

#### Guest:

5. Guest shall be able to register for an account by entering the required details (email, first name, last name, gender, birthdate, city, and password).
6. Guest shall be able to register using their Google account.
7. Guest shall be able to register using their Facebook account.

#### Member & Admin:

8. Member and Admin shall be able to log in by entering email and password.
9. Members registered using Google or Facebook shall be able to log in directly via those platforms.
10. Member and Admin shall be able to log out.

#### Member (Personal Account Management):

11. Member shall be able to view their Account Information page.
12. Member shall be able to edit their profile details (first name, last name, phone number, city).
13. Member shall be able to change their email and password.
14. Member shall be able to delete their account.
15. Member shall be able to download their data.

### C. Book Management and Discovery (Member and Admin)

#### For Members (Lending and Adding Books):

19. Member shall be able to view a “My Books” page.
20. Member shall be able to lend a book by entering the required details (Title, description, Author, Maximum Lend Duration, Book Category, number of pages, Cover Image, borrowing fee).
21. Member shall be able to customize the book description (e.g., bold, italic, insert table, font size).
22. Member shall be able to edit book details.
23. Member shall be able to delete a book.
24. Member shall be able to sort books based on fields (title, author, category, maximum lend duration, due date).
25. Member shall be able to search for specific details on the book page.
26. Member shall be able to specify the number of books to show per page and navigate to a specific page in the books list.
27. Member shall be able to save a book to a wish list.
28. Member shall be able to view the wish list.

#### For Admin (Catalog & Category Management):

29. Admin shall be able to view the Book List.
30. Admin shall be able to sell new books by entering required details (Title, description, Author, List Price, price tiers, category, Cover Image).
31. Admin shall be able to sort books in the Book List based on fields (title, author, category, maximum lend duration, due date, status, price).
32. Admin shall be able to search for specific details in the Book List.
33. Admin shall be able to specify the number of books to show per page and navigate to a specific page.
34. Admin shall be able to edit and update book details.
35. Admin shall be able to delete a book.
36. Admin shall be able to export the books list.

### D. Shopping Cart & Payment (Member)

37. Member shall be able to add books to their shopping cart.
38. Member shall be able to specify the quantity of each book when adding to the cart.
39. Member shall be able to view their shopping cart.
40. Member shall be able to update the quantity of each book in the shopping cart (increase or decrease).
41. Member shall be able to delete a book from the shopping cart.
42. Member shall be able to proceed to checkout and view the checkout details page.
43. Member shall be able to complete a secure checkout by entering required shipping information (full name, phone number, street address, city, and state).
44. Member shall be able to pay using Stripe’s payment gateway by entering payment details.
45. Member shall be able to view a payment confirmation after a successful purchase.

### E. Borrowing & Lending Management (Member)

46. Member shall be able to send a borrow request by entering the required details (full name, email, phone number, location).
47. Member shall be able to view their borrow requests details page.
48. Member shall be able to sort borrow request details in ascending or descending order.
49. Member shall be able to search for specific details in their borrow requests.
50. Member shall be able to specify the number of requests to show per page.
51. Member shall be able to navigate to a specific page in the borrow requests section.
52. Member shall be able to approve a borrowing request.
53. Member shall be able to reject a borrowing request.
54. Member shall be able to mark a book as returned.

### F. Order Management (Admin)

55. Admin shall be able to view order list details.
56. Admin shall be able to filter the orders list page by status (All, In Process, Pending, Completed, Approved).
57. Admin shall be able to sort orders based on fields (ID, name, phone, email, status, total if applicable).
58. Admin shall be able to search for specific details in the orders list.
59. Admin shall be able to specify the number of orders to show per page and navigate to a specific page in the orders list.
60. Admin shall be able to view the order details page (including shipping information, order summary, and payment details).
61. Admin shall be able to edit order shipping information (full name, phone number, street address, city, state, carrier).
62. Admin shall be able to start processing an order.
63. Admin shall be able to start shipping orders.
64. Admin shall be able to cancel an order.

### G. External Authentication & Security (Member)

65. Member shall be able to view External Logins and log in via available external providers.
66. Member shall be able to set up an authenticator application.
67. Member shall be able to reset the authenticator application.

### H. Category and System Administration (Admin)

68. Admin shall be able to view the categories list.
69. Admin shall be able to create a new category.
70. Admin shall be able to edit and update a category.
71. Admin shall be able to delete a category.
72. Admin shall be able to create a new account by entering the required details and selecting the role (admin or member).

Dead Pigeons – Game Platform

Dead Pigeons is a web app that digitizes a weekly number game that’s normally run partly on paper and partly in person. The goal of the app is to make the game easier to manage without locking it into a single, non-scalable setup.

Instead of hard-coding one game configuration, the app is built around game templates.

Admins can create multiple templates with different number variations and rules. A game is always started using one specific template, and if the game is marked as repeatable, the next game is automatically created for the following week using the same template after the draw is completed. This makes the system flexible and scalable as the game grows or changes.

Example Game Rules:
The Draw: 3 numbers are drawn weekly from a physical hat (1-16).
The Tickets: Players choose between 5 and 8 numbers per ticket.
Winning: A win occurs if the 3 winning numbers appear anywhere within the player's chosen sequence (order does not matter).
Revenue: 70% goes to the prize pool (managed manually by admins); 30% goes to the sports facility.

Roles and Permissions
- **Player**: Request membership, deposit funds (MobilePay), purchase/repeat boards, view winning history.
- **Admin**: Full Player CRUD, approve registrations, verify transactions, start games, and enter winning numbers.

Financial Logic (The Balance System)
To ensure financial security, the app utilizes a pre-paid balance system:
Deposit: Players deposit money and provide a MobilePay transaction number.
Verification: The transaction remains Pending until an Admin manually verifies it against MobilePay records.
Purchase: Once approved, players use their balance to buy tickets for active games.

Note: Prize money is handled separately by administrators and is not added to the digital balance.

**Features**

For Players
MyTickets: View active and past tickets. Winning tickets are clearly highlighted.
Wallet: Manage balance, view transaction history, and submit deposit requests.
Auto-Play: Option to purchase ticket subscription for a game, works until its cancelled or there is no more funds to cover the next ticket purchase.
Membership Request: New users can sign up and wait for Admin activation.

For Administrators
Dashboard: Centralized view of pending players and transactions for quick approval.
Game Management: Create "Game Templates" for quick setup and starting new game.
Deadline Enforcement: The system prevents drawing numbers before the actual draw date.
Winner Overview: A clear, timestamped history of all games and winning tickets to assist with manual prize payouts.

⚠️ Known Bugs & Limitations
Email System: Automated emails for password recovery or new account credentials are not currently implemented.
Withdrawals: The "Withdraw Funds" interface is visible but non-functional, as it was not a requirement for the current version.
Prize Calculation: The system does not calculate the specific DKK payout per person, as it must be calculated alongside physical game participants.

🔑 Test Credentials
| Role        | Username / Email | Password         |
| ----------- | ---------------- | ---------------- |
| Admin       | admin@admin.com  | test_admin       |
| Player      | player@test.com  | test_player      |

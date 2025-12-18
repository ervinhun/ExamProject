# Project Overview
The "Dead Pigeons" game involves players choosing a sequence of numbers (1-16) to match a weekly winning sequence of 3 numbers drawn by an administrator. This platform handles the digital side of the game, tracking balances, ticket purchases, and winning boards while co-existing with physical participants.

Key Game Rules:
The Draw: 3 numbers are drawn weekly from a physical hat (1-16).
The Boards: Players choose between 5 and 8 numbers per board.
Winning: A win occurs if the 3 winning numbers appear anywhere within the player's chosen sequence (order does not matter).
Revenue: 70% goes to the prize pool (managed manually by admins); 30% goes to the sports facility.

Roles and Permissions
- **Player**: Request membership, deposit funds (MobilePay), purchase/repeat boards, view winning history.
- **Admin**: Full Player CRUD, approve registrations, verify transactions, start games, and enter winning numbers.
- **Super Admin**: All Admin permissions + the ability to create and manage other Admin accounts.

Financial Logic (The Balance System)
To ensure financial security, the app utilizes a pre-paid balance system:
Deposit: Players deposit money and provide a MobilePay transaction number.
Verification: The transaction remains Pending until an Admin manually verifies it against MobilePay records.
Purchase: Once approved, players use their balance to buy boards based on the unique game templates.

Note: Prize money is handled separately by administrators and is not added to the digital balance.

**Features**

For Players
MyTickets: View active and past tickets. Winning tickets are clearly highlighted.
Wallet: Manage balance, view transaction history, and submit deposit requests.
Auto-Play: Option to repeat boards for $X$ number of weeks.
Membership Request: New users can sign up and wait for Admin activation.

For Administrators
Dashboard: Centralized view of pending players and transactions for quick approval.
Game Management: Create "Game Templates" for quick setup and start new weekly rounds.
Deadline Enforcement: The system prevents entries after Saturday 5:00 PM (Danish Local Time).
Winner Overview: A clear, timestamped history of all games and winning boards to assist with manual prize payouts.

⚠️ Known Bugs & Limitations
Email System: Automated emails for password recovery or new account credentials are not currently implemented.
Withdrawals: The "Withdraw Funds" interface is visible but non-functional, as it was not a requirement for the current version.
Prize Calculation: The system does not calculate the specific DKK payout per person, as it must be calculated alongside physical game participants.

🔑 Test Credentials
| Role        | Username / Email | Password         |
| ----------- | ---------------- | ---------------- |
| Admin       | admin@admin.com  | test_admin       |
| Player      | player@test.com  | test_player      |

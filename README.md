# Chasm
A cAlgo bot experiment, using Gap Trading Strategies

## How does it work?

The BOT accepts 6 parameters:
- LONG Trailing Stop %: Stop Loss percentage for Long positions (default: 8)
- SHORT Trailing Stop %: Stop Loss percentage for Short positions (default: 4)
- Diff ticks: Take profit ticks (default: 2)
- GAP UP Long: if Yes, GAP UP position will trigger a Long position. If no, Short (default: Yes)
- GAP DOWN Long: if Yes, GAP DOWN position will trigger a Long position. If no, Short (default: No)
- N.Contracts: Number of contracts to Buy/Sell

### Bot Logic
The bot is checking for the closing price of previous day and comparing it with the opening price of current day. If opening price is bigger than yesterday's close then we will have a GAP UP. Otherwise, if today's opening price is lower than yesterday's close, then we will have a GAP DOWN. If none of this scenarios is happening, the BOT will stay on standby for the current day.

If the BOT detects a GAP it will behave according to the configuration:

#### GAP UP + Long
One hour after the market's opening time, it will open a LONG position and set a take profit calculated on the value of the Diff Ticks parameter, added to the max price reached today.

#### GAP UP + Short
One hour after the market's opening time, it will open a SHORT position and set a take profit calculated on the value of the Diff Ticks parameter, subtracted from the min price reached today.

#### GAP DOWN + Long
One hour after the market's opening time, it will open a LONG position and set a take profit calculated on the value of the Diff Ticks parameter, applied to the min price reached today.

#### GAP DOWN + Short
One hour after the market's opening time, it will open a SHORT position and set a take profit calculated on the value of the Diff Ticks parameter, subtracted from the min price reached today.


Stop loss is always calculated using the value of LONG Trailing Stop or SHORT Trailing Stop (%) parameters, depending on the actual scenario. 

The n. of contracts is defined in the "N.Contracts" parameter.

## Backtesting Screenshot
![Backtesting Screenshot](https://github.com/antoniocosentino/Chasm/blob/master/screen.jpg)


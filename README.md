# Track A: FACE - Financial Analysis Calculation Engine

Input & Schema Setup

- Define all input variables (theme, revenue size, kitchen size, retraining need, equipment condition). 

--> /input_handler.py
  
- Design database schema (tables for inputs, cost factors, outputs). 
--> /sql.py
  
Calculation Engine

- Write functions to compute costs based on weighted formulas (e.g., retraining employees × staff size, equipment depreciation × replacement rate).
--> /engine_classes.py
  
- Unit test calculations with dummy data. 
--> /engine.py
  
UI Prototype

- Build a form where user selects theme, revenue size, etc. 
--> /main.cs (.NET framework prefferable - c# or java + xml)
  
- Show output as a table (each row = one colored section with calculated numbers). 
--> /output_pipe.cs/py (as you wish, c# for better integration)
  
Expansion

- Add multiple scenario comparisons side by side (e.g., “Good Location” vs “Cloud Kitchen”). 
--> /parallel_analysis.py
  
- Export results to CSV or PDF. 
--> /output_pipe.cs/py (again, depends on you for better integration)

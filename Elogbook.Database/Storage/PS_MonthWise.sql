CREATE PARTITION SCHEME [PS_MonthWise]
    AS PARTITION [PF_MonthlyPartition]
    TO ([FG_Elogbook_Q1_2020], [FG_Elogbook_Q2_2020], [FG_Elogbook_Q3_2020], [FG_Elogbook_Q4_2020], [FG_Elogbook_Q1_2021], [FG_Elogbook_Q2_2021], [FG_Elogbook_Q3_2021], [FG_Elogbook_Q4_2021], [FG_Elogbook_Q1_2022], [FG_Elogbook_Q2_2022], [FG_Elogbook_Q3_2022], [FG_Elogbook_Q4_2022], [PRIMARY]);


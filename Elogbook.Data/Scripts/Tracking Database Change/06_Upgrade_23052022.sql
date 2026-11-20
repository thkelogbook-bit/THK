--23052022
UPDATE par
SET
	par.FactoryID = Grp.FactoryID,
	par.CycleID = Grp.CycleID,
	par.OperationPositionID = Grp.OperationPositionID
FROM dbo.Parameters par WITH(NOLOCK)
INNER JOIN dbo.ParameterGroup Grp WITH(NOLOCK) ON par.ParameterGroupID = Grp.ID
WHERE ParameterGroupID IS NOT NULL
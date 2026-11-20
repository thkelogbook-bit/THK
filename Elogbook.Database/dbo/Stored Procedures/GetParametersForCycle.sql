
CREATE PROCEDURE [dbo].[GetParametersForCycle]
    @CycleID INT,
    @OperationPositionID INT,
    @FactoryID INT,
    @ParameterGroupID INT,
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    -- Danh sách khung giờ theo chu kỳ
    SELECT *
    INTO #TimeLineCycles
    FROM TimeLineCycle
    WHERE CycleID = @CycleID
        AND Active = 1
        AND IsDeleted = 0;

    -- Danh sách ID của các nhóm thông số
    SELECT ID
    INTO #ParameterGroupIDs
    FROM ParameterGroup
    WHERE CycleID = @CycleID
        AND OperationPositionID = @OperationPositionID
        AND FactoryID = @FactoryID
        AND Status = 1
        AND IsDeleted = 0;

    -- Danh sách tất cả thông số theo chu kỳ
    SELECT p.ID, p.ParameterName, p.IsGroup, p.KKSCode, p.UnitMeasure, p.Setting, p.FromRange, p.ToRange, p.IsNumberType,
           a.ID AS AlertID, a.Alert, a.Operator, a.Value, a.Color
    INTO #AllParameters
    FROM Parameters p
    LEFT JOIN AlertDetails a ON p.ID = a.ParameterID
    WHERE p.FactoryID = @FactoryID
        AND p.OperationPositionID = @OperationPositionID
        AND p.CycleID = @CycleID
        AND (p.ParameterGroupID IN (SELECT ID FROM #ParameterGroupIDs) OR p.ParameterGroupID IS NULL)
        AND p.Status = 1
        AND p.IsDeleted = 0;

    -- Nhóm thông số
    IF @ParameterGroupID > 0
    BEGIN
        DELETE FROM #AllParameters
        WHERE IsGroup = 0
            OR (IsGroup = 1 AND ParameterGroupID <> @ParameterGroupID);
    END;

    -- Thông số lẻ
    IF @ParameterGroupID = 0
    BEGIN
        DELETE FROM #AllParameters
        WHERE IsGroup = 1;
    END;

    -- Trả kết quả theo trang
    SELECT *
    FROM #AllParameters
    ORDER BY ID
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Số lượng bản ghi tổng cộng
    SELECT COUNT(*) AS TotalRecords
    FROM #AllParameters;

    DROP TABLE #TimeLineCycles;
    DROP TABLE #ParameterGroupIDs;
    DROP TABLE #AllParameters;
END
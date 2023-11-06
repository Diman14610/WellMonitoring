import CustomStore from 'devextreme/data/custom_store';
import { DataGrid } from 'devextreme-react';
import { Column, FilterRow, Pager } from 'devextreme-react/data-grid';
import 'devextreme/dist/css/dx.light.css';
import * as signalR from "@microsoft/signalr";
import { useEffect, useState } from 'react';
import WellFullInfo from '@/types/WellFullInfo';

export default function DataGridComponent({ jsonDataSource }) {

  return (
    <DataGrid
      className="dx-card wide-card m-2"
      dataSource={jsonDataSource}
      showBorders={false}
      focusedRowEnabled
      columnAutoWidth
      columnHidingEnabled
    >
      <Pager showPageSizeSelector showInfo />
      <FilterRow visible />

      <Column
        dataField="telemetryId"
        caption='Идентификатор телеметрии'
        dataType='number'
        width={130}
      />
      <Column
        dataField="dateTime"
        caption='Дата и время'
        dataType='date'
        width={160}
      />
      <Column
        dataField="contractorName"
        caption='Подрядчик'
      />
      <Column
        dataField="wellName"
        caption='Название скважины'
      />
      <Column
        dataField="depth"
        width={150}
        dataType='number'
        caption='Глубина'
      />
    </DataGrid>
  )
}
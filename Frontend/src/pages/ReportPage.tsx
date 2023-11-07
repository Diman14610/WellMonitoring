import CustomStore from 'devextreme/data/custom_store';
import { DataGrid } from 'devextreme-react';
import { Column, FilterRow, Pager } from 'devextreme-react/data-grid';
import 'devextreme/dist/css/dx.light.css';
import * as signalR from "@microsoft/signalr";
import { useEffect, useRef, useState } from 'react';
import TelemetryInfo from '@/types/TelemetryInfo';
import { connectionString } from '@/config';
import { Button, ButtonGroup } from '@mui/material';

const getConnection = (value: string): string => `${connectionString}${value}`;

export default function ReportPage() {
  const handleErrors = (response) => {
    if (!response.ok) {
      throw Error(response.statusText);
    }
    return response;
  };

  const jsonDataSource = new CustomStore({
    key: 'telemetryId',
    loadMode: 'raw',
    select: [
      'telemetryId',
      'wellName',
      'depth',
      'dateTime',
      'contractorName',
    ],
    load: () => (
      fetch(getConnection('api/v1/telemetry/all'))
        .then(handleErrors)
        .then((response) => response.json())
        .catch((e) => console.error(e))
    ),
  });

  return (
    <div>
      <div className='mx-2'>
        <ButtonGroup variant="outlined" size="small">
          <Button>One</Button>
          <Button>Two</Button>
          <Button>Three</Button>
        </ButtonGroup>
      </div>

      <DataGrid
        className="dx-card wide-card m-2"
        dataSource={jsonDataSource}
        showBorders={false}
        focusedRowEnabled
        columnAutoWidth
        columnHidingEnabled
        repaintChangesOnly={true}
        highlightChanges={true}
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
    </div >
  )
}

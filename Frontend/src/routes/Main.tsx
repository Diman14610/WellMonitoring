import CustomStore from 'devextreme/data/custom_store';
import { DataGrid } from 'devextreme-react';
import { Column, FilterRow, Pager } from 'devextreme-react/data-grid';
import 'devextreme/dist/css/dx.light.css';
import * as signalR from "@microsoft/signalr";
import { useEffect, useRef, useState } from 'react';
import WellFullInfo from '@/types/WellFullInfo';
import DataGridComponent from '@/components/data-grid/DataGridComponent';

const hubConnection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:44355/messages", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets,
  })
  .build();

hubConnection.start().then(function () {
  console.log("Подключение установлено");
}).catch(function (err) {
  console.error(err.toString());
});

export default function MainPage() {
  function handleErrors(response) {
    if (!response.ok) {
      throw Error(response.statusText);
    }
    return response;
  }

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
      fetch('https://localhost:44355/api/v1/telemetry/all')
        .then(handleErrors)
        .then((response) => response.json())
        .catch((e) => console.error(e))
    ),
  });

  useEffect(() => {
    hubConnection.on("newTelemetry", (message: WellFullInfo) => {
      console.log(message);
      jsonDataSource.push([{ type: 'insert', key: 'telemetryId', data: message }]);
    });
  }, []);

  return (
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
  )
}

import CustomStore from 'devextreme/data/custom_store';
import { DataGrid } from 'devextreme-react';
import { Column, FilterRow, GroupPanel, Grouping, Pager } from 'devextreme-react/data-grid';
import 'devextreme/dist/css/dx.light.css';
import * as signalR from "@microsoft/signalr";
import { useEffect, useState } from 'react';
import TelemetryInfo from '../types/TelemetryInfo';
import { getConnection } from '../utils/connectionString';

const hubConnection = new signalR.HubConnectionBuilder()
  .withUrl(getConnection('messages'))
  .build();

export default function MainPage() {
  const [count, setCount] = useState(0);
  const [isConnected, setIsConnected] = useState(false);

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
      fetch(getConnection('api/telemetry/all'))
        .then(handleErrors)
        .then((response) => response.json())
        .catch((e) => console.error(e))
    ),
  });

  useEffect(() => {
    hubConnection.start().then(function () {
      setIsConnected(true);

      hubConnection.on("newTelemetry", (message: TelemetryInfo) => {
        jsonDataSource.push([{ type: 'insert', key: 'telemetryId', data: message }]);
        setCount(i => i += 1);
      });
    }).catch(function (err) {
      setIsConnected(false);
      console.error(err.toString());
    });

    return () => {
      hubConnection.stop();
    }
  }, []);

  return (
    <div>
      <div style={{ alignItems: 'center' }} className='flex gap-2 mx-2'>
        <div className='m-2'>
          <span>Подключение: </span>
          {isConnected ? (<span style={{ color: 'green' }}>в сети</span>) : (<span style={{ color: 'red' }}>не в сети</span>)}
        </div>
        <div>Получено новых: {count}</div>
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
        <Grouping contextMenuEnabled autoExpandAll expandMode="rowClick" />
        <GroupPanel visible />

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

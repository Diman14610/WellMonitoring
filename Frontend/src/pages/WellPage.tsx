import { DataGrid, DateBox } from 'devextreme-react';
import { Column, Export, FilterRow, GroupPanel, Grouping, Pager, Selection } from 'devextreme-react/data-grid';
import 'devextreme/dist/css/dx.light.css';
import { useEffect, useState, useCallback } from 'react';
import axios from 'axios';
import jsPDF from 'jspdf';
import { exportDataGrid } from 'devextreme/pdf_exporter';
import { Button } from '@mui/material';
import { font } from './../assets/font-string';
import { getConnection } from '../utils/connectionString';
import ShortWellInfo from '../types/ShortWellInfo';
import DepthInfo from '../types/DepthInfo';

const exportFormats: Array<"GIF" | "JPEG" | "PDF" | "PNG" | "SVG"> = ['PDF'];

export default function WellPage() {
  const [prevDate, setPrevDate] = useState(new Date());
  const [currentDate, setCurrentDate] = useState(new Date());
  const [isUpdating, setIsUpdating] = useState(false);
  const [source, setSource] = useState<ShortWellInfo[]>([]);

  const loadResources = () => {
    try {
      axios.get(getConnection('api/v1/company/all')).then(async (response) => {
        const { data } = response;
        const wells: ShortWellInfo[] = [];

        for (const company of data) {
          const url = new URL(getConnection(`api/v1/well/depth/company/${company}`))
          url.searchParams.append('fromDate', new Date(prevDate).toLocaleString('en-US'));
          url.searchParams.append('toDate', new Date(currentDate).toLocaleString('en-US'));

          const tasks = [
            axios.get(getConnection(`api/v1/well/company/${company}`)),
            axios.get(getConnection(`api/v1/well/active/company/${company}`)),
            axios.get<DepthInfo[]>(url.toString())
          ];
          const [wellResponse, activeWellResponse, wellsDepth] = await Promise.all(tasks);

          const wellData = wellResponse.data;
          const activeWellData = activeWellResponse.data;

          for (const item of wellData) {
            wells.push({
              CompanyName: company,
              Flag: false,
              WellName: item,
              Depth: wellsDepth.data.find(a => a.wellName === item)?.score ?? 0
            });
          }

          for (const item of activeWellData) {
            const foundWell = wells.find((g) => g.CompanyName === company && g.WellName === item);
            if (foundWell) {
              foundWell.Flag = true;
            }
          }
        }

        setSource(() => [...wells]);
        setIsUpdating(false);
      });
    } catch (error) {
      console.error(error);
      setIsUpdating(false);
    }
  }

  useEffect(() => {
    loadResources();
  }, []);

  const onExporting = useCallback((e: any) => {
    const doc = new jsPDF();

    doc.addFileToVFS('ciril-normal.ttf', font);
    doc.addFont('ciril-normal.ttf', 'ciril', 'normal');
    doc.setFont('ciril');

    exportDataGrid({
      jsPDFDocument: doc,
      component: e.component,
      indent: 5,
    }).then(() => {
      doc.save('Companies.pdf');
    });
  }, []);

  const dateTimeLabel = { 'aria-label': 'Date Time' };

  return (
    <>
      {source.length === 0 ? (
        <span className='mx-5'>Загрузка...</span>
      ) : (
        <div>
          <div
            style={{
              display: 'flex',
              gap: 65,
            }}
            className='mx-2'
          >
            <div className="dx-field">
              <div className="dx-field-label">Начало</div>
              <div className="dx-field-value">
                <DateBox
                  defaultValue={prevDate}
                  inputAttr={dateTimeLabel}
                  disabled={isUpdating}
                  type="datetime"
                  onValueChange={(e) => setPrevDate(new Date(e))}
                />
              </div>
            </div>
            <div className="dx-field">
              <div className="dx-field-label">Конец</div>
              <div className="dx-field-value">
                <DateBox
                  defaultValue={currentDate}
                  inputAttr={dateTimeLabel}
                  disabled={isUpdating}
                  type="datetime"
                  onValueChange={(e) => setCurrentDate(new Date(e))}
                />
              </div>
            </div>
            <div>
              <Button
                disabled={isUpdating}
                onClick={() => {
                  setIsUpdating(true);
                  loadResources();
                }}
              >
                Применить
              </Button>
            </div>
          </div>

          <DataGrid
            className="dx-card wide-card m-2"
            dataSource={source}
            showBorders={false}
            columnAutoWidth
            columnHidingEnabled
            repaintChangesOnly={true}
            highlightChanges={true}
            onExporting={onExporting}
            disabled={isUpdating}
          >
            <Pager showPageSizeSelector showInfo />
            <Grouping contextMenuEnabled autoExpandAll expandMode="rowClick" />
            <GroupPanel visible />
            <FilterRow visible />
            <Export enabled formats={exportFormats} allowExportSelectedData />
            <Selection mode="multiple" />

            <Column
              groupIndex={0}
              dataField="CompanyName"
              caption='Подрядчик'
            />
            <Column
              dataField="WellName"
              caption='Название скважины'
            />
            <Column
              dataField="Depth"
              caption='Глубина'
            />
            <Column
              dataField="Flag"
              dataType='boolean'
              caption='Активный?'
            />
          </DataGrid>
        </div>
      )}
    </>
  )
}

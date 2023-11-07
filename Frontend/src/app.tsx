import { ScrollToTop } from '@/components/scroll-to-top'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { loadMessages, locale } from 'devextreme/localization';
import ruMessages from "devextreme/localization/messages/ru.json"
import MainPage from './pages/MainPage'
import Base from './routes/MainLayout';
import NotFound from './pages/NotFound';
import ReportPage from './pages/ReportPage';

export default function App() {
  loadMessages(ruMessages);
  locale(navigator.language);

  const basename = import.meta.env.BASE_URL

  return (
    <BrowserRouter basename={basename}>
      <ScrollToTop>
        <Routes>
          <Route path='/' element={<Base />}>
            <Route path='telemetry' element={<MainPage />} />
            <Route path='report' element={<ReportPage />} />
            <Route path="*" element={<NotFound />} />
          </Route>
        </Routes>
      </ScrollToTop>
    </BrowserRouter>
  )
}

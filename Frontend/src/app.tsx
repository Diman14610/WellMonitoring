import { ScrollToTop } from '@/components/scroll-to-top'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { formatMessage, loadMessages, locale } from 'devextreme/localization';
import ruMessages from "devextreme/localization/messages/ru.json"
import MainPage from './routes/Main'
import Base from './routes/MainLayout';
import NotFound from './routes/NotFound';

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
            <Route path="*" element={<NotFound />} />
          </Route>
        </Routes>
      </ScrollToTop>
    </BrowserRouter>
  )
}

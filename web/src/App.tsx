import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store';
import { ThemeProvider } from './hooks/useTheme';
import Index from './pages/Index';
import Game from './pages/Game';

function App() {
  return (
    <Provider store={store}>
      <ThemeProvider>
        <Router>
          <div className="cover-container">
            <Routes>
              <Route path="/" element={<Index />} />
              <Route path="/game/:instance" element={<Game />} />
            </Routes>
          </div>
        </Router>
      </ThemeProvider>
    </Provider>
  );
}

export default App;


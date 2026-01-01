import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store';
import Index from './pages/Index';
import Game from './pages/Game';
import './App.css';

function App() {
  return (
    <Provider store={store}>
      <Router>
        <div className="cover-container">
          <Routes>
            <Route path="/" element={<Index />} />
            <Route path="/game/:instance" element={<Game />} />
          </Routes>
        </div>
      </Router>
    </Provider>
  );
}

export default App;


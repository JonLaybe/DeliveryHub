import './App.scss'
import HeaderComponent from './shared/layout/header/HeaderComponent'
import RoutingComponent from './components/routing/RoutingComponent'
import { BrowserRouter } from 'react-router-dom'
import { SearchProvider } from './context/SearchContext'
import ReactModal from 'react-modal'
import FiltersComponent from './components/filters/FiltersComponent'

function App() {
  ReactModal.setAppElement('#root');

  return (
    <BrowserRouter>
      <SearchProvider>
        <HeaderComponent />
        <div className='wrapper'>
          <RoutingComponent />
        </div>
      </SearchProvider>
    </BrowserRouter>
  )
}

export default App

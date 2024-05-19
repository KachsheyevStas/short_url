import { useEffect, useState} from 'react';
import { Link } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';

import { linkActions } from '_store';
import { AddEdit } from './AddEdit';
export { List };


const Row = ({row})=>{
    const baseUrl = `${process.env.REACT_APP_API_URL}`

    const dispatch = useDispatch();
    const { id, originalUrl, createDate, shortUrl,countClick, isDeleting } = row;
    const [count, setCount] = useState(countClick);
    return (
        <tr key={row.id}>
                            <td>{originalUrl}</td>
                            <td>{createDate}</td>
                            <td>
                                <a onClick={()=>setCount(count+1)} href={`${baseUrl}/${shortUrl}`} target="_blank">{`${baseUrl}/${shortUrl}`}</a>
                             </td>

                            <td>{count}</td>
                            
                            <td style={{ whiteSpace: 'nowrap' }}>
                                <Link to={`edit/${id}`} className="btn btn-sm btn-primary me-1">Edit</Link>
                                <button onClick={() => dispatch(linkActions.delete(id))} className="btn btn-sm btn-danger" style={{ width: '60px' }} disabled={isDeleting}>
                                    {isDeleting 
                                        ? <span className="spinner-border spinner-border-sm"></span>
                                        : <span>Delete</span>
                                    }
                                </button>
                            </td>
        </tr>
      );
}

function List() {
    const links = useSelector(x => x.links.list);
    const dispatch = useDispatch();
    
    useEffect(() => {
        dispatch(linkActions.getAll());
    }, [dispatch]);

    return (
        <div>
            <h1>Short Url</h1>
            <AddEdit></AddEdit>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th style={{ width: '30%' }}>Original Url</th>
                        <th style={{ width: '30%' }}>Created</th>
                        <th style={{ width: '30%' }}>Short URL</th>
                        <th style={{ width: '30%' }}>All Clicks</th>
                        <th style={{ width: '10%' }}></th>
                    </tr>
                </thead>
                <tbody>
                {links?.value?.map(link => <Row row={link} key={link.id}></Row>)}
                    {links?.loading &&
                        <tr>
                            <td colSpan="4" className="text-center">
                                <span className="spinner-border spinner-border-lg align-center"></span>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    );
}
